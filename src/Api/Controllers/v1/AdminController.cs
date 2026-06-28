using Application.Features.Admin.Queries;
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
}

