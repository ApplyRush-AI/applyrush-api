using Application.Features.Enums.Queries;
using DTO.Enums.Job;
using DTO.Enums.JobApplication;
using DTO.Enums.Notification;
using DTO.Enums.Profile.Education;
using DTO.Enums.Profile.EeoData;
using DTO.Enums.Profile.UserProfile;
using DTO.Enums.Resume;
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

    // Notification

    [HttpGet("notification-types")]
    public async Task<IActionResult> GetNotificationTypes(CancellationToken ct)
        => Ok(await Mediator.Send(new GetEnumValuesQuery(typeof(NotificationType)), ct));
}
