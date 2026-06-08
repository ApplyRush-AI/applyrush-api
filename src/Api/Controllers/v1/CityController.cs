using Application.Features.Cities.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

public class CityController : ApiControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> List([FromQuery] string? search, [FromQuery] int? limit)
    {
        return Ok(await Mediator.Send(new CityListGetQuery(search, limit)));
    }
}
