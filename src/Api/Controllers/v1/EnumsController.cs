using Application.Features.Enums.Queries;
using DTO.Enums.Job;
using DTO.Enums.JobApplication;
using DTO.Enums.Notification;
using DTO.Enums.Profile.Education;
using DTO.Enums.Profile.EeoData;
using DTO.Enums.Profile.UserProfile;
using DTO.Enums.Resume;
using DTO.Enums.Subscription;
using DTO.Enums.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

[AllowAnonymous]
public sealed class EnumsController : ApiControllerBase
{
    // Job

    [HttpGet("experience-levels")]
    public async Task<IActionResult> GetExperienceLevels(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(ExperienceLevel)), ct));

    [HttpGet("job-types")]
    public async Task<IActionResult> GetJobTypes(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(JobType)), ct));

    [HttpGet("work-models")]
    public async Task<IActionResult> GetWorkModels(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(WorkModel)), ct));

    [HttpGet("match-tiers")]
    public async Task<IActionResult> GetMatchTiers(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(MatchTier)), ct));

    // Job Application

    [HttpGet("application-statuses")]
    public async Task<IActionResult> GetApplicationStatuses(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(ApplicationStatus)), ct));

    // Profile

    [HttpGet("degree-types")]
    public async Task<IActionResult> GetDegreeTypes(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(DegreeType)), ct));

    [HttpGet("location-modes")]
    public async Task<IActionResult> GetLocationModes(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(LocationMode)), ct));

    [HttpGet("genders")]
    public async Task<IActionResult> GetGenders(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(Gender)), ct));

    // EEO

    [HttpGet("disability-statuses")]
    public async Task<IActionResult> GetDisabilityStatuses(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(DisabilityStatus)), ct));

    [HttpGet("races")]
    public async Task<IActionResult> GetRaces(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(Race)), ct));

    [HttpGet("sexual-orientations")]
    public async Task<IActionResult> GetSexualOrientations(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(SexualOrientation)), ct));

    [HttpGet("veteran-statuses")]
    public async Task<IActionResult> GetVeteranStatuses(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(VeteranStatus)), ct));

    [HttpGet("work-authorizations")]
    public async Task<IActionResult> GetWorkAuthorizations(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(WorkAuthorization)), ct));

    // Resume

    [HttpGet("ai-rewrite-actions")]
    public async Task<IActionResult> GetAiRewriteActions(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(AiRewriteAction)), ct));

    [HttpGet("resume-file-types")]
    public async Task<IActionResult> GetResumeFileTypes(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(ResumeFileType)), ct));

    [HttpGet("issue-severities")]
    public async Task<IActionResult> GetIssueSeverities(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(IssueSeverity)), ct));

    [HttpGet("resume-analysis-grades")]
    public async Task<IActionResult> GetResumeAnalysisGrades(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(ResumeAnalysisGrade)), ct));

    [HttpGet("resume-analysis-statuses")]
    public async Task<IActionResult> GetResumeAnalysisStatuses(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(ResumeAnalysisStatus)), ct));

    [HttpGet("resume-parse-statuses")]
    public async Task<IActionResult> GetResumeParseStatuses(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(ResumeParseStatus)), ct));

    [HttpGet("tailoring-statuses")]
    public async Task<IActionResult> GetTailoringStatuses(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(TailoringStatus)), ct));

    [HttpGet("tailoring-work-modes")]
    public async Task<IActionResult> GetTailoringWorkModes(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(TailoringWorkMode)), ct));

    // Custom Resume Style

    [HttpGet("paper-sizes")]
    public async Task<IActionResult> GetPaperSizes(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(PaperSize)), ct));

    [HttpGet("resume-templates")]
    public async Task<IActionResult> GetResumeTemplates(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(ResumeTemplate)), ct));

    [HttpGet("accent-color-scopes")]
    public async Task<IActionResult> GetAccentColorScopes(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(AccentColorScope)), ct));

    [HttpGet("resume-font-families")]
    public async Task<IActionResult> GetResumeFontFamilies(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(ResumeFontFamily)), ct));

    [HttpGet("resume-date-formats")]
    public async Task<IActionResult> GetResumeDateFormats(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(ResumeDateFormat)), ct));

    [HttpGet("bullet-icons")]
    public async Task<IActionResult> GetBulletIcons(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(BulletIcon)), ct));

    [HttpGet("resume-header-alignments")]
    public async Task<IActionResult> GetResumeHeaderAlignments(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(ResumeHeaderAlignment)), ct));

    [HttpGet("education-display-orders")]
    public async Task<IActionResult> GetEducationDisplayOrders(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(EducationDisplayOrder)), ct));

    [HttpGet("skills-layouts")]
    public async Task<IActionResult> GetSkillsLayouts(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(SkillsLayout)), ct));

    // Notification

    [HttpGet("notification-types")]
    public async Task<IActionResult> GetNotificationTypes(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(NotificationType)), ct));

    // Subscription

    [HttpGet("subscription-plans")]
    public async Task<IActionResult> GetSubscriptionPlans(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(SubscriptionPlan)), ct));

    [HttpGet("billing-intervals")]
    public async Task<IActionResult> GetBillingIntervals(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(BillingInterval)), ct));
}
