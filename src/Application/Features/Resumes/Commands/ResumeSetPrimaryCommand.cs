using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Interfaces.Request;
using Application.Common.Interfaces.Request.Handlers;
using Application.Features.Educations.Data;
using Application.Features.WorkExperiences.Data;
using Application.Features.WorkExperiences.WorkExperienceBullets.Data;
using Domain.Entities.Profiles.Educations;
using Domain.Entities.Profiles.UserProfiles;
using Domain.Entities.Profiles.UserSkills;
using Domain.Entities.Profiles.WorkExpeciences;
using Domain.Entities.Profiles.WorkExpeciences.WorkExperienceBullets;
using Domain.Entities.Profiles.WorkExperiences;
using Domain.Entities.Resumes;
using Domain.Entities.Resumes.Resume;
using DTO.Enums;
using DTO.Enums.Profile.Education;
using DTO.Resumes;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Resumes.Commands;

public sealed record ResumeSetPrimaryCommand(
    int ResumeId,
    bool UpdatePersonal = false,
    bool UpdateSkills = false,
    bool UpdateWorkExperience = false,
    bool UpdateEducation = false) : ICommand<ResumeSetPrimaryResponse>;

public sealed class ResumeSetPrimaryCommandHandler : ICommandHandler<ResumeSetPrimaryCommand, ResumeSetPrimaryResponse>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;

    public ResumeSetPrimaryCommandHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<ResumeSetPrimaryResponse> Handle(ResumeSetPrimaryCommand command, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId!.Value;

        var resume = await _dbContext.Resume
            .FirstOrDefaultAsync(r => r.Id == command.ResumeId && r.UserId == userId && r.Status == Status.Active, cancellationToken)
            ?? throw NotFoundException.New<Resume>();

        var currentPrimary = await _dbContext.Resume
            .Where(r => r.UserId == userId && r.IsPrimary && r.Status == Status.Active && r.Id != command.ResumeId)
            .ToListAsync(cancellationToken);

        foreach (var r in currentPrimary)
            r.ClearPrimary();

        resume.SetPrimary();

        var updateAny = command.UpdatePersonal || command.UpdateSkills || command.UpdateWorkExperience || command.UpdateEducation;
        if (updateAny && resume.ParsedData is { } parsed)
            await UpdateUserProfileAsync(userId, parsed, command, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new ResumeSetPrimaryResponse { PrimaryResumeId = resume.Id };
    }

    private async Task UpdateUserProfileAsync(int userId, ResumeParseData parsed, ResumeSetPrimaryCommand command, CancellationToken cancellationToken)
    {
        var profile = await _dbContext.UserProfile
            .Include(p => p.Skills)
            .Include(p => p.WorkExperiences)
            .Include(p => p.Educations)
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

        if (profile == null)
        {
            profile = UserProfile.Create(userId);
            _dbContext.UserProfile.Add(profile);
        }

        if (command.UpdatePersonal)
        {
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

        if (command.UpdateSkills)
            ReplaceSkills(profile, parsed.Skills);

        if (command.UpdateWorkExperience)
            ReplaceWorkExperiences(profile, parsed.Experience);

        if (command.UpdateEducation)
            ReplaceEducations(profile, parsed.Education);

        _dbContext.UserProfile.Update(profile);
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
        var order = 0;
        foreach (var item in items)
        {
            if (string.IsNullOrWhiteSpace(item.JobTitle) || string.IsNullOrWhiteSpace(item.Company))
                continue;

            var exp = WorkExperience.Create(new WorkExperienceCreateData(
                profile.Id,
                item.JobTitle,
                item.Company,
                null,
                item.Location,
                ParseDate(item.StartDate) ?? DateOnly.MinValue,
                ParseDate(item.EndDate),
                item.IsCurrent,
                item.Summary), order++);

            var bulletOrder = 0;
            foreach (var bullet in item.Bullets.Where(b => !string.IsNullOrWhiteSpace(b)))
                exp.Bullets.Add(WorkExperienceBullet.Create(new WorkExperienceBulletCreateData(0, bullet), bulletOrder++));

            profile.WorkExperiences.Add(exp);
        }
    }

    private static void ReplaceEducations(UserProfile profile, IReadOnlyList<ResumeParseEducationItem> items)
    {
        profile.Educations.Clear();
        var order = 0;
        foreach (var item in items)
        {
            if (string.IsNullOrWhiteSpace(item.School))
                continue;

            profile.Educations.Add(Education.Create(new EducationCreateData(
                profile.Id,
                item.School,
                item.Major,
                ParseDegreeType(item.Degree),
                item.Gpa,
                ParseDate(item.StartDate),
                ParseDate(item.EndDate),
                item.IsCurrent), order++));
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
}
