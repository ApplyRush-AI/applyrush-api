using Application.Common.Interfaces;
using Application.Common.Interfaces.Services;
using Domain.Entities.Jobs.JobListings;
using Domain.Entities.Jobs.UserJobMatches;
using Domain.Entities.Profiles.UserProfiles;
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
            .Include(j => j.JobFunctions)
            .FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);

        if (job == null)
            return existing!;

        var profile = await _dbContext.UserProfile
            .Include(p => p.Skills)
            .Include(p => p.WorkExperiences)
            .Include(p => p.PreferredJobFunctions)
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        var data = ComputeScore(userId, profile, job);

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

    public async Task<IReadOnlyList<UserJobMatch>> ComputeForAllUsersAsync(
        int jobId,
        CancellationToken cancellationToken)
    {
        var job = await _dbContext.JobListing
            .AsNoTracking()
            .Include(j => j.JobFunctions)
            .FirstOrDefaultAsync(j => j.Id == jobId, cancellationToken);

        if (job == null)
            return [];

        var profiles = await _dbContext.UserProfile
            .Include(p => p.Skills)
            .Include(p => p.WorkExperiences)
            .Include(p => p.PreferredJobFunctions)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return profiles
            .Select(p => UserJobMatch.Create(ComputeScore(p.UserId, p, job)))
            .ToList();
    }

    private UserJobMatchUpsertData ComputeScore(
        int userId,
        UserProfile? profile,
        JobListing job)
    {
        decimal skillScore = 0;
        decimal experienceScore = 0;
        decimal titleScore = 0;
        decimal industryScore = 0;
        var matchedSkills = new List<string>();

        if (profile != null)
        {
            if (!string.IsNullOrEmpty(job.RequiredSkills))
            {
                var requiredSkills = JsonSerializer.Deserialize<List<string>>(job.RequiredSkills) ?? [];
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

            var now = DateOnly.FromDateTime(_dateTime.Now);
            var totalYears = profile.WorkExperiences
                .Sum(w =>
                {
                    var end = w.EndDate ?? now;
                    return (end.Year - w.StartDate.Year) + (end.Month - w.StartDate.Month) / 12.0m;
                });

            experienceScore = job.YearsRequired.HasValue
                ? job.YearsRequired.Value == 0 ? 100 : Math.Min(100, totalYears / job.YearsRequired.Value * 100)
                : 70;

            titleScore = ComputeTitleScore(profile, job.Title, job.JobFunctions);

            if (job.WorkModel == WorkModel.Remote)
            {
                industryScore = profile.OpenToRemote ? 100 : 60;
            }
            else if (!string.IsNullOrEmpty(profile.City) && !string.IsNullOrEmpty(job.Location))
            {
                var locationLower = job.Location.ToLowerInvariant();
                industryScore = locationLower.Contains(profile.City.ToLowerInvariant()) ? 100
                    : (!string.IsNullOrEmpty(profile.Country) && locationLower.Contains(profile.Country.ToLowerInvariant())) ? 70
                    : 40;
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

        return new UserJobMatchUpsertData(
            userId, job.Id,
            Math.Round(overallScore, 2),
            Math.Round(experienceScore, 2),
            Math.Round(skillScore, 2),
            Math.Round(titleScore, 2),
            Math.Round(industryScore, 2),
            JsonSerializer.Serialize(matchedSkills),
            tier,
            _dateTime.Now);
    }

    private static decimal ComputeTitleScore(
        UserProfile profile,
        string jobTitle,
        ICollection<JobListingJobFunction> jobFunctions)
    {
        if (jobFunctions.Any(jf => profile.PreferredJobFunctions.Any(pf => pf.JobFunctionId == jf.JobFunctionId)))
            return 100;

        if (string.IsNullOrEmpty(profile.Title)) return 50;

        var titleWords = jobTitle.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var headlineWords = profile.Title.ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries).ToHashSet();
        var overlap = titleWords.Count(w => headlineWords.Contains(w));
        return titleWords.Length == 0 ? 50 : Math.Min(100, (decimal)overlap / titleWords.Length * 100);
    }
}
