using Application.Features.Extension.Commands;
using Application.Features.Extension.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

public class ExtensionController : ApiControllerBase
{
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        return Ok(await Mediator.Send(new ExtensionProfileGetQuery()));
    }

    [HttpPost("autofill")]
    public async Task<IActionResult> Autofill([FromBody] ExtensionAutofillCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [HttpPost("answer")]
    public async Task<IActionResult> GenerateAnswer([FromBody] ExtensionAnswerGenerateCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [HttpGet("credits")]
    public async Task<IActionResult> GetCredits()
    {
        return Ok(await Mediator.Send(new ExtensionCreditGetQuery()));
    }
}
