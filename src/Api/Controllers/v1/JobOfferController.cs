using Application.Features.JobOffers.Commands;
using Application.Features.JobOffers.Queries;
using AutoMapper;
using DTO.JobOffers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

public class JobOfferController : ApiControllerBase
{
    private readonly IMapper _mapper;

    public JobOfferController(IMapper mapper)
    {
        _mapper = mapper;
    }

    [HttpPost]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Create([FromBody] JobOfferCreateRequest request, CancellationToken ct)
    {
        var command = _mapper.Map<JobOfferCreateCommand>(request);
        return Ok(await Mediator.Send(command, ct));
    }

    [HttpGet]
    public async Task<IActionResult> GetFeed([FromQuery] JobOfferFeedQuery feed, CancellationToken ct)
    {
        return Ok(await Mediator.Send(feed, ct));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        return Ok(await Mediator.Send(new JobOfferGetByIdQuery(id), ct));
    }

    [HttpGet("industries")]
    [AllowAnonymous]
    public async Task<IActionResult> GetIndustries(CancellationToken ct)
    {
        return Ok(await Mediator.Send(new JobOfferIndustriesGetQuery(), ct));
    }

    [HttpPost("rebuild-index")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> RebuildIndex(CancellationToken ct)
    {
        await Mediator.Send(new JobOfferInitiateSearchIndexRebuildCommand(), ct);
        return Accepted();
    }

    [HttpPost("{id:int}/hide")]
    public async Task<IActionResult> Hide(int id, CancellationToken ct)
    {
        await Mediator.Send(new JobOfferHideCommand(id), ct);
        return Ok();
    }

    [HttpDelete("{id:int}/hide")]
    public async Task<IActionResult> Unhide(int id, CancellationToken ct)
    {
        await Mediator.Send(new JobOfferUnhideCommand(id), ct);
        return Ok();
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
