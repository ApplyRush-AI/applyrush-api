using Application.Common.Interfaces;
using Application.Common.Interfaces.Services;
using Application.Common.Services;
using Application.Features.Authentication.Data;
using Application.Identity;
using Domain.Entities.Profiles.Educations;
using Domain.Entities.Profiles.UserProfiles;
using Domain.Entities.Profiles.UserSkills;
using Domain.Entities.Profiles.WorkExpeciences;
using Domain.Entities.Profiles.WorkExpeciences.WorkExperienceBullets;
using Domain.Entities.Profiles.WorkExperiences;
using Domain.Entities.Resumes;
using Domain.Entities.Resumes.Resume;
using Worker.Consumers.Resumes.Data;
using Domain.Entities.User;
using Domain.Interfaces;
using DTO.Enums.Job;
using DTO.Enums.Media;
using DTO.Enums.Profile.Education;
using DTO.MessageBroker.Messages.Resumes;
using Infrastructure.Identity;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Worker.Consumers.Resumes;

public sealed class ResumeUploadedMessageConsumer : IConsumer<ResumeUploadedMessage>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IResumeParseService _parseService;
    private readonly IApiService _apiService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IIdentityContextAccessor _identityContextAccessor;
    private readonly ILogger<ResumeUploadedMessageConsumer> _logger;

    public ResumeUploadedMessageConsumer(
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork,
        IResumeParseService parseService,
        IApiService apiService,
        UserManager<ApplicationUser> userManager,
        IIdentityContextAccessor identityContextAccessor,
        ILogger<ResumeUploadedMessageConsumer> logger)
    {
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
        _parseService = parseService;
        _apiService = apiService;
        _userManager = userManager;
        _identityContextAccessor = identityContextAccessor;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ResumeUploadedMessage> context)
    {
        var ct = context.CancellationToken;
        var resumeId = context.Message.ResumeId;
        _identityContextAccessor.IdentityContext = new IdentityContextCustom(new UserInfoById(context.Message.UserId));

        var resume = await _dbContext.Resume
            .FirstOrDefaultAsync(r => r.Id == resumeId, ct);
        if (resume == null)
        {
            _logger.LogWarning("Resume {ResumeId}: not found — skipping parse", resumeId);
            return;
        }

        var mediaItem = resume.Media.Items.FirstOrDefault();
        if (mediaItem == null)
        {
            _logger.LogWarning("Resume {ResumeId}: no media item found — skipping parse", resumeId);
            resume.MarkParseFailed();
            await _unitOfWork.SaveChangesAsync(ct);
            return;
        }

        ResumeParseData parsed;
        try
        {
            var ext = Path.GetExtension(mediaItem.Url);
            await using var fileStream = await _apiService.DownloadFile(MediaEntityType.Resume, resumeId, mediaItem.Id)
                ?? throw new InvalidOperationException($"Failed to download resume file for ResumeId={resumeId}");
            parsed = await _parseService.ParseAsync(fileStream, mediaItem.Name, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Resume {ResumeId}: parsing failed", resumeId);
            resume.MarkParseFailed();
            await _unitOfWork.SaveChangesAsync(ct);
            return;
        }

        if (resume.IsPrimary && !IsEmpty(parsed))
            await PersistAsync(parsed, context.Message.UserId, ct);
        else if (IsEmpty(parsed))
            _logger.LogInformation("Resume {ResumeId}: parser returned no data — skipping persistence", resumeId);

        resume.SetParsedData(parsed);
        resume.MarkParseCompleted();
        await _unitOfWork.SaveChangesAsync(ct);
    }

    private async Task PersistAsync(ResumeParseData parsed, int userId, CancellationToken ct)
    {
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user == null) return;

        if (string.IsNullOrWhiteSpace(user.FirstName) && !string.IsNullOrWhiteSpace(parsed.FirstName))
            user.FirstName = parsed.FirstName;
        if (string.IsNullOrWhiteSpace(user.LastName) && !string.IsNullOrWhiteSpace(parsed.LastName))
            user.LastName = parsed.LastName;
        if (string.IsNullOrWhiteSpace(user.PhoneNumber) && !string.IsNullOrWhiteSpace(parsed.Phone))
            user.PhoneNumber = parsed.Phone;

        var profile = await _dbContext.UserProfile
            .Include(p => p.Skills)
            .Include(p => p.WorkExperiences)
            .Include(p => p.Educations)
            .FirstOrDefaultAsync(p => p.UserId == userId, ct);

        if (profile == null)
        {
            profile = UserProfile.Create(userId);
            _dbContext.UserProfile.Add(profile);
        }

        UpdateProfileFields(profile, parsed);
        ReplaceSkills(profile, parsed.Skills);
        ReplaceWorkExperiences(profile, parsed.Experience);
        ReplaceEducations(profile, parsed.Education);
    }

    private static void UpdateProfileFields(UserProfile profile, ResumeParseData parsed)
    {
        // Always overwrite — if the parser didn't return a value, set it to null
        profile.SetFirstName(parsed.FirstName);
        profile.SetLastName(parsed.LastName);
        profile.SetEmail(parsed.Email);
        profile.SetPhone(parsed.Phone);
        profile.SetLinkedInUrl(parsed.LinkedInUrl);
        profile.SetGitHubUrl(parsed.GitHubUrl);
        profile.SetWebsiteUrl(parsed.WebsiteUrl);
        profile.SetTitle(parsed.Title);
        profile.SetBio(parsed.Bio);
        profile.SetCountry(parsed.Country);
        profile.SetCity(parsed.City);
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

