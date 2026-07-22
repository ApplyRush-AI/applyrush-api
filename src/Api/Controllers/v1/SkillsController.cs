using Application.Features.Skills.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

public sealed class SkillsController : ApiControllerBase
{
    // "suggestions" is the route the client already integrates against; the bare route is kept as an alias.
    [HttpGet]
    [HttpGet("suggestions")]
    public async Task<IActionResult> Search([FromQuery] string? query, [FromQuery] int take, CancellationToken ct)
    {
        return Ok(await Mediator.Send(new SkillSearchQuery(query, take <= 0 ? 20 : take), ct));
    }
}
