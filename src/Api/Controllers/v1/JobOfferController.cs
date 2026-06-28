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
    public async Task<IActionResult> GetFeed([FromQuery] JobOfferFeedQuery query, CancellationToken ct)
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
