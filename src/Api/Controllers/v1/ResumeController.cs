using Application.Features.Resumes.Commands;
using Application.Features.Resumes.Queries;
using Application.Features.ResumeTailorings.Commands;
using Domain.Entities.Resumes;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

public class ResumeController : ApiControllerBase
{
    [HttpPost("generate-custom")]
    public async Task<IActionResult> GenerateCustom([FromBody] ResumeTailoringGenerateCommand command)
    {
        return Ok(await Mediator.Send(command));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        return Ok(await Mediator.Send(new ResumeGetByIdQuery(id)));
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] ResumeUploadCommand request)
    {
        return Ok(await Mediator.Send(request));
    }

    [HttpGet("{id:int}/download")]
    public async Task<IActionResult> Download(int id)
    {
        var file = await Mediator.Send(new ResumeDownloadQuery(id));
        return File(file);
    }

    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        return Ok(await Mediator.Send(new ResumeListGetQuery()));
    }

    [HttpPut("{id:int}/name")]
    public async Task<IActionResult> Rename(int id, [FromBody] string name)
    {
        await Mediator.Send(new ResumeRenameCommand(id, name));
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await Mediator.Send(new ResumeDeleteCommand(id));
        return NoContent();
    }

    [HttpPut("{id:int}/set-primary")]
    public async Task<IActionResult> SetPrimary(
        int id,
        [FromQuery] bool updatePersonal = false,
        [FromQuery] bool updateSkills = false,
        [FromQuery] bool updateWorkExperience = false,
        [FromQuery] bool updateEducation = false)
    {
        return Ok(await Mediator.Send(new ResumeSetPrimaryCommand(id, updatePersonal, updateSkills, updateWorkExperience, updateEducation)));
    }

    [HttpPut("{id:int}/parsed-data")]
    public async Task<IActionResult> UpdateParsedData(int id, [FromBody] ResumeParseData parsedData)
    {
        await Mediator.Send(new ResumeUpdateParsedDataCommand(id, parsedData));
        return NoContent();
    }
}

