using Application.Features.Credits.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

[Route("api/v{version:apiVersion}/billing/credits")]
public class CreditController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetCurrentCredits()
    {
        return Ok(await Mediator.Send(new CreditGetCurrentQuery()));
    }
}
