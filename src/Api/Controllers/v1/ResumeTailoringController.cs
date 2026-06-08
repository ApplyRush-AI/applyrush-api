using Application.Features.ResumeTailorings.Commands;
using Application.Features.ResumeTailorings.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

public class ResumeTailoringController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ResumeTailoringCreateCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        return Ok(await Mediator.Send(new ResumeTailoringGetByIdQuery(id)));
    }

    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        return Ok(await Mediator.Send(new ResumeTailoringListGetQuery()));
    }

    [HttpPost("{id:int}/download")]
    public async Task<IActionResult> Download(int id)
    {
        var bytes = await Mediator.Send(new ResumeTailoringDownloadCommand(id));
        return File(bytes, "application/pdf", $"resume-tailoring-{id}.pdf");
    }

    [HttpPut("{id:int}/content")]
    public async Task<IActionResult> UpdateContent(int id, [FromBody] ResumeTailoringContentUpdateCommand command)
    {
        return Ok(await Mediator.Send(command with { Id = id }));
    }

    [HttpPost("{id:int}/ai-rewrite")]
    public async Task<IActionResult> AiRewrite(int id, [FromBody] ResumeTailoringAiRewriteCommand command)
    {
        return Ok(await Mediator.Send(command with { Id = id }));
    }
}
