using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using Domain.Entities.Jobs.JobListings;
using DTO.JobOffers;
using DTO.Resumes;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Application.Features.CustomResumes.Queries;

public sealed record CustomResumeGapAnalysisQuery(int JobId) : IQuery<CustomResumeGapAnalysisResponse>;

public sealed class CustomResumeGapAnalysisQueryHandler
    : IQueryHandler<CustomResumeGapAnalysisQuery, CustomResumeGapAnalysisResponse>
{
    // Keeps the "missing keywords" chip row focused; the job text can mention far more than is useful to show.
    private const int MaxMissingSkills = 15;

    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IMatchScoringService _matchScoringService;
    private readonly IJobSkillExtractor _skillExtractor;

    public CustomResumeGapAnalysisQueryHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IMatchScoringService matchScoringService,
        IJobSkillExtractor skillExtractor)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _matchScoringService = matchScoringService;
        _skillExtractor = skillExtractor;
    }

    public async Task<CustomResumeGapAnalysisResponse> Handle(CustomResumeGapAnalysisQuery query, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var job = await _dbContext.JobListing
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == query.JobId, cancellationToken)
            ?? throw new NotFoundException(nameof(JobListing), query.JobId);

        var userSkills = await _dbContext.UserProfile
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .SelectMany(p => p.Skills.Select(s => s.Name))
            .ToListAsync(cancellationToken);

        // Reuse the same score already shown on the job card; compute lazily if it doesn't exist yet
        // (no credit is consumed — match scoring is free, unlike the AI tailoring flow).
        var match = await _dbContext.UserJobMatch
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.UserId == userId && m.JobId == query.JobId, cancellationToken)
            ?? await _matchScoringService.ComputeAndSaveAsync(userId, query.JobId, cancellationToken);

        var jobSkills = ResolveJobSkills(job);
        var userSkillSet = userSkills
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Select(s => s.Trim().ToLowerInvariant())
            .ToHashSet();

        var matched = jobSkills.Where(s => userSkillSet.Contains(s.ToLowerInvariant())).ToList();
        var missing = jobSkills.Where(s => !userSkillSet.Contains(s.ToLowerInvariant())).Take(MaxMissingSkills).ToList();

        return new CustomResumeGapAnalysisResponse
        {
            MatchScore = match?.OverallScore ?? 0,
            Scores = new MatchScoresResponse
            {
                Experience = match?.ExperienceScore ?? 0,
                Skills = match?.SkillScore ?? 0,
                Title = match?.TitleScore ?? 0,
                Industry = match?.IndustryScore ?? 0
            },
            MatchedSkills = matched,
            MissingSkills = missing
        };
    }

    // The job's skill set: its structured RequiredSkills (rarely present) unioned with skills the extractor
    // finds in the free-text. Matched vs missing are both derived from this one set so the FE's "X / Y" is consistent.
    private IReadOnlyList<string> ResolveJobSkills(JobListing job)
    {
        var explicitSkills = ParseRequiredSkills(job.RequiredSkills);
        var jobText = string.Join("\n", new[] { job.Title, job.Description, job.Requirements, job.Responsibilities }
            .Where(s => !string.IsNullOrWhiteSpace(s)));
        return _skillExtractor.ExtractSkills(jobText, explicitSkills);
    }

    private static IReadOnlyList<string> ParseRequiredSkills(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return [];
        try
        {
            return JsonSerializer.Deserialize<List<string>>(json) ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
    }
}
