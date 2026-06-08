using Application.Features.Settings.JobPreferences.Commands;
using Application.Features.Settings.JobPreferences.Queries;
using Application.Features.Settings.NotificationPreferences.Commands;
using Application.Features.Settings.NotificationPreferences.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

public class SettingsController : ApiControllerBase
{
    [HttpGet("notifications")]
    public async Task<IActionResult> GetNotificationPreferences()
    {
        return Ok(await Mediator.Send(new NotificationPreferenceGetQuery()));
    }

    [HttpPut("notifications")]
    public async Task<IActionResult> UpdateNotificationPreferences([FromBody] NotificationPreferenceUpdateCommand request)
    {
        await Mediator.Send(request);
        return NoContent();
    }

    [HttpGet("job-preferences")]
    public async Task<IActionResult> GetJobPreferences()
    {
        return Ok(await Mediator.Send(new JobPreferenceGetQuery()));
    }

    [HttpPut("job-preferences")]
    public async Task<IActionResult> UpdateJobPreferences([FromBody] JobPreferenceUpdateCommand request)
    {
        await Mediator.Send(request);
        return NoContent();
    }
}
