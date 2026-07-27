using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Common.Interfaces.Services;
using Application.Common.Services;
using Application.Features.Resumes.Data;
using Domain.Entities.Profiles.EeoDatas;
using Domain.Entities.Profiles.Educations;
using Domain.Entities.Profiles.UserProfiles;
using Domain.Entities.Profiles.UserSkills;
using Domain.Entities.Profiles.WorkExpeciences;
using Domain.Entities.Profiles.WorkExpeciences.WorkExperienceBullets;
using Domain.Entities.Profiles.WorkExperiences;
using Domain.Entities.Resumes;
using Domain.Entities.User;
using DTO.Enums.Media;
using DTO.Enums.Profile.Education;
using DTO.Enums.Profile.EeoData;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Resumes.Commands;

public sealed record ResumeParseCommand(int ResumeId, int UserId) : ICommand;

public sealed class ResumeParseCommandHandler : ICommandHandler<ResumeParseCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IResumeParseService _parseService;
    private readonly IApiService _apiService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<ResumeParseCommandHandler> _logger;

    public ResumeParseCommandHandler(
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        IResumeParseService parseService,
        IApiService apiService,
        UserManager<ApplicationUser> userManager,
        ILogger<ResumeParseCommandHandler> logger)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _parseService = parseService;
        _apiService = apiService;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task Handle(ResumeParseCommand command, CancellationToken cancellationToken)
    {
        var resume = await _dbContext.Resume
            .FirstOrDefaultAsync(r => r.Id == command.ResumeId, cancellationToken);
        if (resume == null)
        {
            _logger.LogWarning("Resume {ResumeId}: not found — skipping parse", command.ResumeId);
            return;
        }

        var mediaItem = resume.Media.Items.FirstOrDefault();
        if (mediaItem == null)
        {
            _logger.LogWarning("Resume {ResumeId}: no media item found — skipping parse", command.ResumeId);
            resume.MarkParseFailed();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return;
        }

        var parsed = await ParseAsync(resume.Id, mediaItem.Name, mediaItem.Id, cancellationToken);
        if (parsed == null)
        {
            resume.MarkParseFailed();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return;
        }

        if (resume.IsPrimary && !IsEmpty(parsed))
            await PersistAsync(parsed, command.UserId, cancellationToken);
        else if (IsEmpty(parsed))
            _logger.LogInformation("Resume {ResumeId}: parser returned no data — skipping persistence", resume.Id);

        resume.SetParsedData(parsed);
        resume.MarkParseCompleted();
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<ResumeParseData?> ParseAsync(int resumeId, string fileName, Guid mediaItemId, CancellationToken cancellationToken)
    {
        try
        {
            await using var fileStream = await _apiService.DownloadFile(MediaEntityType.Resume, resumeId, mediaItemId)
                ?? throw new InvalidOperationException($"Failed to download resume file for ResumeId={resumeId}");
            return await _parseService.ParseAsync(fileStream, fileName, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Resume {ResumeId}: parsing failed", resumeId);
            return null;
        }
    }

    private async Task PersistAsync(ResumeParseData parsed, int userId, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (user == null) return;

        FillUserFields(user, parsed);

        var profile = await _dbContext.UserProfile
            .Include(p => p.Skills)
            .Include(p => p.WorkExperiences)
            .Include(p => p.Educations)
            .Include(p => p.EeoData)
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        if (profile == null)
        {
            profile = UserProfile.Create(userId);
            _dbContext.UserProfile.Add(profile);
        }

        FillProfileFields(profile, parsed);

        // Collections are populated only when empty, so re-uploading a resume never wipes a list the
        // user has since edited.
        if (!profile.Skills.Any())
            ReplaceSkills(profile, parsed.Skills);
        if (!profile.WorkExperiences.Any())
            ReplaceWorkExperiences(profile, parsed.Experience);
        if (!profile.Educations.Any())
            ReplaceEducations(profile, parsed.Education);

        await FillEeoAsync(profile, parsed, cancellationToken);
    }

    private static void FillUserFields(ApplicationUser user, ResumeParseData parsed)
    {
        if (string.IsNullOrWhiteSpace(user.FirstName) && !string.IsNullOrWhiteSpace(parsed.FirstName))
            user.FirstName = parsed.FirstName;
        if (string.IsNullOrWhiteSpace(user.LastName) && !string.IsNullOrWhiteSpace(parsed.LastName))
            user.LastName = parsed.LastName;
        if (string.IsNullOrWhiteSpace(user.PhoneNumber) && !string.IsNullOrWhiteSpace(parsed.Phone))
            user.PhoneNumber = parsed.Phone;
    }

    // Fill empty fields only — a value the user already has is never overwritten by a later resume.
    private static void FillProfileFields(UserProfile profile, ResumeParseData parsed)
    {
        if (string.IsNullOrWhiteSpace(profile.FirstName)) profile.SetFirstName(parsed.FirstName);
        if (string.IsNullOrWhiteSpace(profile.LastName)) profile.SetLastName(parsed.LastName);
        if (string.IsNullOrWhiteSpace(profile.Email)) profile.SetEmail(parsed.Email);
        if (string.IsNullOrWhiteSpace(profile.Phone)) profile.SetPhone(parsed.Phone);
        if (string.IsNullOrWhiteSpace(profile.LinkedInUrl)) profile.SetLinkedInUrl(parsed.LinkedInUrl);
        if (string.IsNullOrWhiteSpace(profile.GitHubUrl)) profile.SetGitHubUrl(parsed.GitHubUrl);
        if (string.IsNullOrWhiteSpace(profile.WebsiteUrl)) profile.SetWebsiteUrl(parsed.WebsiteUrl);
        if (string.IsNullOrWhiteSpace(profile.Title)) profile.SetTitle(parsed.Title);
        if (string.IsNullOrWhiteSpace(profile.Bio)) profile.SetBio(parsed.Bio);
        if (string.IsNullOrWhiteSpace(profile.Country)) profile.SetCountry(parsed.Country);
        if (string.IsNullOrWhiteSpace(profile.City)) profile.SetCity(parsed.City);
    }

    private async Task FillEeoAsync(UserProfile profile, ResumeParseData parsed, CancellationToken cancellationToken)
    {
        var workAuthorization = MapWorkAuthorization(parsed.WorkAuthorization);
        if (workAuthorization == null && parsed.SponsorshipNeeded == null)
            return;

        if (profile.EeoData == null)
        {
            var eeoData = EeoData.Create(profile.Id);
            eeoData.FillMissingFromResume(workAuthorization, parsed.SponsorshipNeeded);
            await _dbContext.EeoData.AddAsync(eeoData, cancellationToken);
        }
        else
        {
            profile.EeoData.FillMissingFromResume(workAuthorization, parsed.SponsorshipNeeded);
        }
    }

    // Only work authorization is parsed from a resume; demographic EEO fields are never inferred.
    private static WorkAuthorization? MapWorkAuthorization(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var v = value.ToLowerInvariant();

        if (v.Contains("citizen")) return WorkAuthorization.Citizen;
        if (v.Contains("permanent resident") || v.Contains("green card")) return WorkAuthorization.PermanentResident;
        if (v.Contains("student") || v.Contains("f-1") || v.Contains("f1") || v.Contains("opt")) return WorkAuthorization.StudentVisa;
        if (v.Contains("visa") || v.Contains("h1b") || v.Contains("h-1b") || v.Contains("authorized")) return WorkAuthorization.WorkVisa;

        return WorkAuthorization.Other;
    }

    private static void ReplaceSkills(UserProfile profile, IReadOnlyList<string> skills)
    {
        profile.Skills.Clear();
        var order = 0;
        foreach (var name in skills.Where(s => !string.IsNullOrWhiteSpace(s)))
            profile.Skills.Add(UserSkill.Create(profile.Id, name, order++));
    }

    private static void ReplaceWorkExperiences(UserProfile profile, IReadOnlyList<ResumeParseExperienceItem> items)
    {
        profile.WorkExperiences.Clear();
        if (items.Count == 0) return;
        var order = 0;
        foreach (var item in items)
        {
            if (string.IsNullOrWhiteSpace(item.JobTitle) || string.IsNullOrWhiteSpace(item.Company))
                continue;

            var data = new ParsedWorkExperienceInsertData(
                profile.Id,
                item.JobTitle,
                item.Company,
                item.Location,
                ParseDate(item.StartDate) ?? DateOnly.MinValue,
                ParseDate(item.EndDate),
                item.IsCurrent,
                item.Summary,
                item.Bullets);

            var exp = WorkExperience.Create(data, order++);
            var bulletOrder = 0;
            foreach (var bullet in item.Bullets.Where(b => !string.IsNullOrWhiteSpace(b)))
                exp.Bullets.Add(WorkExperienceBullet.Create(new ParsedBulletInsertData(0, bullet), bulletOrder++));
            profile.WorkExperiences.Add(exp);
        }
    }

    private static void ReplaceEducations(UserProfile profile, IReadOnlyList<ResumeParseEducationItem> items)
    {
        profile.Educations.Clear();
        if (items.Count == 0) return;
        var order = 0;
        foreach (var item in items)
        {
            if (string.IsNullOrWhiteSpace(item.School))
                continue;

            var data = new ParsedEducationInsertData(
                profile.Id,
                item.School,
                item.Major,
                ParseDegreeType(item.Degree),
                item.Gpa,
                GpaScale.FourPoint, // resumes don't state a scale; default to 4.0, the user can change it
                ParseDate(item.StartDate),
                ParseDate(item.EndDate),
                item.IsCurrent);

            profile.Educations.Add(Education.Create(data, order++));
        }
    }

    private static DateOnly? ParseDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        if (DateOnly.TryParseExact(value, "yyyy-MM", out var d)) return d;
        if (DateOnly.TryParseExact(value, "yyyy", out var y)) return y;
        return null;
    }

    private static DegreeType ParseDegreeType(string? degree)
    {
        if (string.IsNullOrWhiteSpace(degree)) return DegreeType.Other;
        return degree.ToLowerInvariant() switch
        {
            var s when s.Contains("bachelor") || s.StartsWith("b.s") || s.StartsWith("b.a") || s == "bs" || s == "ba" => DegreeType.Bachelor,
            var s when s.Contains("master") || s.StartsWith("m.s") || s.StartsWith("m.a") || s == "ms" || s == "ma" || s == "mba" => DegreeType.Master,
            var s when s.Contains("doctor") || s.Contains("phd") || s == "ph.d" => DegreeType.Doctorate,
            var s when s.Contains("associate") => DegreeType.Associate,
            var s when s.Contains("certificate") || s.Contains("cert") => DegreeType.Certificate,
            _ => DegreeType.Other
        };
    }

    private static bool IsEmpty(ResumeParseData r) =>
        string.IsNullOrWhiteSpace(r.FirstName) && string.IsNullOrWhiteSpace(r.LastName)
        && string.IsNullOrWhiteSpace(r.Email) && string.IsNullOrWhiteSpace(r.Phone)
        && r.Skills.Count == 0 && r.Experience.Count == 0 && r.Education.Count == 0;
}
