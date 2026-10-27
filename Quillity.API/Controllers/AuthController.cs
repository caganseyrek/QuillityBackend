using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quillity.Application.Auth;

namespace Quillity.API.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterCommand registerCommand)
    {
        await _mediator.Send(registerCommand);
        return StatusCode(Common.Response.StatusCodes.CREATED, new { message = "Registered successfully" });
    }

    [HttpPost]
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult>LoginAsync([FromBody] LoginCommand loginCommand)
    {
        var loginResult = await _mediator.Send(loginCommand);
        Response.Cookies.Append("atq", loginResult.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(15),
        });
        Response.Cookies.Append("rtq", loginResult.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7),
        });
        return StatusCode(Common.Response.StatusCodes.OK, new { message = "Logged in successfully" });
    }

    [HttpPost]
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        Response.Cookies.Append("atq", "", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddMinutes(15),
        });
        return StatusCode(Common.Response.StatusCodes.OK, new { Message = "Logged out successfully" });
    }

    [HttpPost]
    [Authorize]
    [HttpPost("verify-email/{token}")]
    public async Task<IActionResult> EmailVerificationResponseAsync(string token)
    {
        await _mediator.Send(new EmailVerificationCommand { EmailVerificationToken = token });
        return StatusCode(Common.Response.StatusCodes.OK, new { message = "Email verified successfully" });
    }

    [HttpPost]
    [Authorize]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPasswordRequestAsync()
    {
        return Ok();
    }

    [HttpPost]
    [Authorize]
    [HttpPost("reset-password/{token}")]
    public async Task<IActionResult> ResetPasswordProcessAsync(string token)
    {
        return Ok();
    }

    [HttpPost]
    [Authorize]
    [HttpPost("refresh")]
    public async Task<IActionResult> RenewTokenAsync()
    {
        return Ok();
    } 
}
