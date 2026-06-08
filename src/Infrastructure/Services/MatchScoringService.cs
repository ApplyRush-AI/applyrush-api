using Application.Common.Interfaces;
using Application.Common.Interfaces.Services;
using Domain.Entities.Jobs.UserJobMatches;
using Domain.Interfaces;
using DTO.Enums.Job;
using Infrastructure.Services.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Infrastructure.Services;

public sealed class MatchScoringService : IMatchScoringService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTime _dateTime;

    public MatchScoringService(IApplicationDbContext dbContext, IUnitOfWork unitOfWork, IDateTime dateTime)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _dateTime = dateTime;
    }

    public async Task<UserJobMatch> ComputeAndSaveAsync(
        int userId,
        int jobId,
        CancellationToken cancellationToken)
    {
        var existing = await _dbContext.UserJobMatch
            .FirstOrDefaultAsync(m => m.UserId == userId && m.JobId == jobId, cancellationToken);

        var job = await _dbContext.JobListing
            .FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);

        if (job == null)
            return existing!;

        var profile = await _dbContext.UserProfile
            .Include(p => p.Skills)
            .Include(p => p.WorkExperiences)
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        decimal skillScore = 0;
        decimal experienceScore = 0;
        decimal titleScore = 0;
        decimal industryScore = 0;
        var matchedSkills = new List<string>();

        if (profile != null)
        {
            // Skill matching (40%)
            if (!string.IsNullOrEmpty(job.RequiredSkills))
            {
                var requiredSkills = JsonSerializer.Deserialize<List<string>>(job.RequiredSkills) ?? new List<string>();
                var userSkillNames = profile.Skills.Select(s => s.Name.ToLowerInvariant()).ToHashSet();
                matchedSkills = requiredSkills.Where(s => userSkillNames.Contains(s.ToLowerInvariant())).ToList();
                skillScore = requiredSkills.Count > 0
                    ? Math.Min(100, (decimal)matchedSkills.Count / requiredSkills.Count * 100)
                    : 50;
            }
            else
            {
                skillScore = 50;
            }

            // Experience matching (30%)
            var totalYears = profile.WorkExperiences
                .Sum(w =>
                {
                    var start = w.StartDate;
                    var end = w.EndDate ?? DateOnly.FromDateTime(_dateTime.Now);
                    return (end.Year - start.Year) + (end.Month - start.Month) / 12.0m;
                });

            if (job.YearsRequired.HasValue)
            {
                experienceScore = job.YearsRequired.Value == 0
                    ? 100
                    : Math.Min(100, totalYears / job.YearsRequired.Value * 100);
            }
            else
            {
                experienceScore = 70;
            }

            // Title matching (20%) - simple keyword overlap
            titleScore = ComputeTitleScore(profile, job.Title);

            // Location matching (10%)
            if (job.WorkModel == WorkModel.Remote)
            {
                industryScore = profile.OpenToRemote ? 100 : 60;
            }
            else if (!string.IsNullOrEmpty(profile.City) && !string.IsNullOrEmpty(job.Location))
            {
                var locationLower = job.Location.ToLowerInvariant();
                if (locationLower.Contains(profile.City.ToLowerInvariant()))
                    industryScore = 100;
                else if (!string.IsNullOrEmpty(profile.Country) && locationLower.Contains(profile.Country.ToLowerInvariant()))
                    industryScore = 70;
                else
                    industryScore = 40;
            }
            else
            {
                industryScore = 50;
            }
        }

        var overallScore = skillScore * 0.4m + experienceScore * 0.3m + titleScore * 0.2m + industryScore * 0.1m;
        var tier = overallScore >= 80 ? MatchTier.StrongMatch
            : overallScore >= 60 ? MatchTier.GoodMatch
            : MatchTier.FairMatch;

        var data = new UserJobMatchUpsertData(
            userId, jobId,
            Math.Round(overallScore, 2),
            Math.Round(experienceScore, 2),
            Math.Round(skillScore, 2),
            Math.Round(titleScore, 2),
            Math.Round(industryScore, 2),
            JsonSerializer.Serialize(matchedSkills),
            tier,
            _dateTime.Now);

        if (existing != null)
        {
            existing.Update(data);
        }
        else
        {
            existing = UserJobMatch.Create(data);
            _dbContext.UserJobMatch.Add(existing);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return existing;
    }

    private static decimal ComputeTitleScore(
        Domain.Entities.Profiles.UserProfiles.UserProfile profile,
        string jobTitle)
    {
        if (string.IsNullOrEmpty(profile.Title)) return 50;

        var titleWords = jobTitle.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var headlineWords = profile.Title.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries).ToHashSet();
        var overlap = titleWords.Count(w => headlineWords.Contains(w));
        return titleWords.Length == 0 ? 50 : Math.Min(100, (decimal)overlap / titleWords.Length * 100);
    }
}
