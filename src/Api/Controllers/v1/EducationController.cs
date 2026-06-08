using Application.Features.Educations.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

public class EducationController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EducationCreateCommand request)
    {
        return Ok(await Mediator.Send(request));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] EducationUpdateCommand request)
    {
        await Mediator.Send(request with { Id = id });
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await Mediator.Send(new EducationDeleteCommand(id));
        return NoContent();
    }

    [HttpPut("reorder")]
    public async Task<IActionResult> Reorder([FromBody] EducationReorderCommand request)
    {
        await Mediator.Send(request);
        return NoContent();
    }
}
