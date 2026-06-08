using Application.Features.ResumeAnalyses.Commands;
using Application.Features.ResumeAnalyses.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

public class ResumeAnalysisController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create()
    {
        return Ok(await Mediator.Send(new ResumeAnalysisCreateCommand()));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        return Ok(await Mediator.Send(new ResumeAnalysisGetByIdQuery(id)));
    }

    [HttpGet("latest")]
    public async Task<IActionResult> GetLatest()
    {
        return Ok(await Mediator.Send(new ResumeAnalysisGetLatestQuery()));
    }

    [HttpPost("{id:int}/fix")]
    public async Task<IActionResult> FixIssue(int id, [FromBody] ResumeAnalysisFixIssueCommand command)
    {
        return Ok(await Mediator.Send(command with { Id = id }));
    }

    [HttpPost("{id:int}/fix-all")]
    public async Task<IActionResult> FixAll(int id)
    {
        return Ok(await Mediator.Send(new ResumeAnalysisFixAllCommand(id)));
    }
}
