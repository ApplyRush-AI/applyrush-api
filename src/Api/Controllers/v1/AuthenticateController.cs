using Application.Features.Authentication.Commands.GoogleAuth;
using Application.Features.Authentication.Commands.Login;
using Application.Features.Authentication.Commands.ResendCode;
using Application.Features.Authentication.Commands.SignOut;
using Application.Features.Authentication.Commands.TokenRefresh;
using Application.Features.Authentication.Commands.VerifyEmail.Commands;
using Application.Features.Users.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.v1;

public class AuthenticateController : ApiControllerBase
{
    [AllowAnonymous]
    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] UserCreateCommand request)
    {
        return Ok(await Mediator.Send(request));
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Authenticate([FromBody] LoginCommand request)
    {
        return Ok(await Mediator.Send(request));
    }

    [AllowAnonymous]
    [HttpPost("google")]
    public async Task<IActionResult> GoogleAuth([FromBody] GoogleAuthCommand request)
    {
        return Ok(await Mediator.Send(request));
    }

    [HttpPost("signout")]
    public async Task<IActionResult> SignOut([FromBody] SignOutCommand request)
    {
        await Mediator.Send(request);
        return NoContent();
    }

    [AllowAnonymous]
    [HttpPut("verify")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailCommand request)
    {
        await Mediator.Send(request);
        return Ok();
    }

    [AllowAnonymous]
    [HttpPut("verify/resend-code")]
    public async Task<IActionResult> ResendVerificationCodeForCientApp([FromBody] ResendVeirifcationCommand request)
    {
        await Mediator.Send(request);
        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenCommand request)
    {
        return Ok(await Mediator.Send(request));
    }
}
