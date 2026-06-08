using Application.Features.WorkExperiences.Commands;
using Application.Features.WorkExperiences.WorkExperienceBullets.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

public class WorkExperienceController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] WorkExperienceCreateCommand request)
    {
        return Ok(await Mediator.Send(request));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] WorkExperienceUpdateCommand request)
    {
        await Mediator.Send(request with { Id = id });
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await Mediator.Send(new WorkExperienceDeleteCommand(id));
        return NoContent();
    }

    [HttpPut("reorder")]
    public async Task<IActionResult> Reorder([FromBody] WorkExperienceReorderCommand request)
    {
        await Mediator.Send(request);
        return NoContent();
    }

    [HttpPost("{workExperienceId:int}/bullets")]
    public async Task<IActionResult> CreateBullet(int workExperienceId, [FromBody] WorkExperienceBulletCreateCommand request)
    {
        return Ok(await Mediator.Send(request with { WorkExperienceId = workExperienceId }));
    }

    [HttpPut("{workExperienceId:int}/bullets/{bulletId:int}")]
    public async Task<IActionResult> UpdateBullet(int workExperienceId, int bulletId, [FromBody] WorkExperienceBulletUpdateCommand request)
    {
        await Mediator.Send(request with { WorkExperienceId = workExperienceId, BulletId = bulletId });
        return NoContent();
    }

    [HttpDelete("{workExperienceId:int}/bullets/{bulletId:int}")]
    public async Task<IActionResult> DeleteBullet(int workExperienceId, int bulletId)
    {
        await Mediator.Send(new WorkExperienceBulletDeleteCommand(workExperienceId, bulletId));
        return NoContent();
    }
}
