using Application.Features.Profiles.Commands;
using Application.Features.Profiles.EeoDatas.Commands;
using Application.Features.Profiles.Queries;
using Application.Features.Profiles.UserSkills.Commands;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

public class ProfileController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        return Ok(await Mediator.Send(new ProfileGetCurrentQuery()));
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] ProfileUpdateCommand request)
    {
        await Mediator.Send(request);
        return NoContent();
    }

    [HttpPut("onboarding")]
    public async Task<IActionResult> UpdateOnboarding([FromBody] ProfileOnboardingUpdateCommand request)
    {
        await Mediator.Send(request);
        return NoContent();
    }

    [HttpPut("job-preferences")]
    public async Task<IActionResult> UpdateJobPreferences([FromBody] ProfileJobPreferencesUpdateCommand request)
    {
        await Mediator.Send(request);
        return NoContent();
    }

    [HttpPut("skills")]
    public async Task<IActionResult> ReplaceSkills([FromBody] UserSkillsReplaceCommand request)
    {
        await Mediator.Send(request);
        return NoContent();
    }

    [HttpPut("eeo")]
    public async Task<IActionResult> UpdateEeo([FromBody] EeoDataUpdateCommand request)
    {
        await Mediator.Send(request);
        return NoContent();
    }

    [HttpGet("completion")]
    public async Task<IActionResult> GetCompletion()
    {
        return Ok(await Mediator.Send(new ProfileCompletionGetQuery()));
    }
}
