using Application.Features.Sessions.Commands;
using Application.Features.Sessions.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

public class SessionController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List()
    {
        return Ok(await Mediator.Send(new SessionListGetQuery()));
    }

    [HttpDelete]
    public async Task<IActionResult> Revoke([FromBody] SessionRevokeCommand request)
    {
        await Mediator.Send(request);
        return NoContent();
    }
}
