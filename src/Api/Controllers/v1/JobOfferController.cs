using Application.Features.JobOffers.Commands;
using Application.Features.JobOffers.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

public class JobOfferController : ApiControllerBase
{
    [HttpPost("feed")]
    public async Task<IActionResult> GetFeed([FromBody] JobOfferFeedQuery query, CancellationToken ct)
    {
        return Ok(await Mediator.Send(query, ct));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        return Ok(await Mediator.Send(new JobOfferGetByIdQuery(id), ct));
    }

    [HttpPost("rebuild-index")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> RebuildIndex(CancellationToken ct)
    {
        await Mediator.Send(new JobOfferInitiateSearchIndexRebuildCommand(), ct);
        return Accepted();
    }

    [HttpGet("sync/status")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetSyncStatus(CancellationToken ct)
    {
        return Ok(await Mediator.Send(new JobSyncStatusGetQuery(), ct));
    }

    [HttpPost("sync")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> TriggerSync(CancellationToken ct)
    {
        await Mediator.Send(new JobSyncCommand(), ct);
        return Accepted();
    }
}
