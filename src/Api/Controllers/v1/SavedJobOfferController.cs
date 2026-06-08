using Application.Features.SavedJobOffers.Commands;
using Application.Features.SavedJobOffers.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

public class SavedJobOfferController : ApiControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        return Ok(await Mediator.Send(new SavedJobOfferListGetQuery()));
    }

    [HttpPost("{jobId:int}")]
    public async Task<IActionResult> Save(int jobId)
    {
        await Mediator.Send(new JobOfferSaveCommand(jobId));
        return NoContent();
    }

    [HttpDelete("{jobId:int}")]
    public async Task<IActionResult> Unsave(int jobId)
    {
        await Mediator.Send(new JobOfferUnsaveCommand(jobId));
        return NoContent();
    }
}
