using Application.Features.JobFunctions.Commands;
using Application.Features.JobFunctions.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

public class JobFunctionController : ApiControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> List()
    {
        return Ok(await Mediator.Send(new JobFunctionListGetQuery()));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] JobFunctionCreateCommand request)
    {
        return Ok(await Mediator.Send(request));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] JobFunctionUpdateCommand request)
    {
        await Mediator.Send(request with { Id = id });
        return NoContent();
    }
}
