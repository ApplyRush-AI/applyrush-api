using Application.Features.Admin.Queries;
using Application.Features.JobOffers.Commands;
using Application.Features.Notifications.Commands;
using Application.Features.Users.Commands;
using DTO.Enums.Subscription;
using DTO.Enums.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

[Authorize(Roles = UserRole.Administrator)]
public class AdminController : ApiControllerBase
{
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        return Ok(await Mediator.Send(new AdminDashboardGetQuery()));
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        return Ok(await Mediator.Send(new AdminUserListGetQuery(search, page, pageSize)));
    }

    [HttpGet("users/{id:int}")]
    public async Task<IActionResult> GetUserDetail(int id)
    {
        return Ok(await Mediator.Send(new AdminUserDetailGetQuery(id)));
    }

    [HttpGet("subscriptions")]
    public async Task<IActionResult> GetSubscriptions([FromQuery] SubscriptionPlan? plan)
    {
        return Ok(await Mediator.Send(new AdminSubscriptionListGetQuery(plan)));
    }

    [HttpGet("job-sync/status")]
    public async Task<IActionResult> GetJobSyncStatus()
    {
        return Ok(await Mediator.Send(new AdminJobSyncStatusGetQuery()));
    }

    [HttpPost("job-sync/run")]
    public async Task<IActionResult> RunJobSync([FromQuery] int pages = 1)
    {
        await Mediator.Send(new JobSyncInitiateCommand(pages));
        return Accepted();
    }

    [HttpPost("job-match/run")]
    public async Task<IActionResult> RunJobMatchAll(CancellationToken ct)
    {
        await Mediator.Send(new JobMatchRebuildAllCommand(), ct);
        return Accepted();
    }

    [HttpPost("search/reindex")]
    public async Task<IActionResult> ReindexSearch(CancellationToken ct)
    {
        var result = await Mediator.Send(new JobOfferRebuildSearchIndexCommand(), ct);

        if (result.Succeeded)
            return Ok(result);

        // 503 rather than 500: the rebuild logic is fine, the search cluster it depends on is not.
        return StatusCode(StatusCodes.Status503ServiceUnavailable, new ProblemDetails
        {
            Status = StatusCodes.Status503ServiceUnavailable,
            Title = "Search index rebuild failed",
            Detail = result.Error,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.4"
        });
    }

    [HttpPost("job-functions/link-all")]
    public async Task<IActionResult> LinkJobFunctionsToAll(CancellationToken ct)
    {
        var updated = await Mediator.Send(new JobFunctionLinkAllCommand(), ct);
        return Ok(new { updated });
    }

    [HttpGet("search/health")]
    public async Task<IActionResult> GetSearchHealth(CancellationToken ct)
    {
        return Ok(await Mediator.Send(new AdminSearchHealthGetQuery(), ct));
    }
}

