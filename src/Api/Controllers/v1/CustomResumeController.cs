using Application.Features.CustomResumes.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

public class CustomResumeController : ApiControllerBase
{
    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] CustomResumeGenerateCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [HttpPost("rewrite")]
    public async Task<IActionResult> Rewrite([FromBody] CustomResumeRewriteCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [HttpPost("download")]
    public async Task<IActionResult> Download([FromBody] CustomResumeDownloadCommand command)
    {
        var bytes = await Mediator.Send(command);
        return File(bytes, "application/pdf", "custom-resume.pdf");
    }
}
