using Application.Features.Account.Commands;
using Application.Features.Account.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

public class AccountController : ApiControllerBase
{
    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] AccountDeleteCommand request)
    {
        await Mediator.Send(request);
        return NoContent();
    }

    [HttpGet("export")]
    public async Task<IActionResult> Export()
    {
        var file = await Mediator.Send(new AccountDataExportQuery());
        return File(file);
    }
}
