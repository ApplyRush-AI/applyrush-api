using Application.Features.JobApplications.Commands;
using Application.Features.JobApplications.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

public class JobApplicationController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        return Ok(await Mediator.Send(new JobApplicationListGetQuery()));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] JobApplicationCreateCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [HttpPatch("{jobId:int}/stage")]
    public async Task<IActionResult> UpdateStage(int jobId, [FromBody] JobApplicationUpdateStageCommand command)
    {
        return Ok(await Mediator.Send(command with { JobId = jobId }));
    }

    [HttpPatch("{jobId:int}/note")]
    public async Task<IActionResult> UpdateNote(int jobId, [FromBody] JobApplicationUpdateNoteCommand command)
    {
        return Ok(await Mediator.Send(command with { JobId = jobId }));
    }

    [HttpDelete("{jobId:int}")]
    public async Task<IActionResult> Delete(int jobId)
    {
        await Mediator.Send(new JobApplicationDeleteCommand(jobId));
        return NoContent();
    }
}
