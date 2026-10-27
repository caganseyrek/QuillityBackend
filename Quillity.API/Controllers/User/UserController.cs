using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quillity.Application.User.Core.Commands;
using Quillity.Application.User.Core.Queries;

namespace Quillity.API.Controllers.User;

[Authorize]
[ApiController]
[Route("user")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserDetailsAsync()
    {
        var userDetails = await _mediator.Send(new GetUserDetailsQuery());
        return StatusCode(Common.Response.StatusCodes.OK, userDetails);
    }

    [HttpGet("preferences")]
    public async Task<IActionResult> GetUserPreferencesAsync()
    {
        var userPreferences = await _mediator.Send(new GetUserPreferencesQuery());
        return StatusCode(Common.Response.StatusCodes.OK, userPreferences);
    }

    [HttpGet("invitations")]
    public async Task<IActionResult> GetWorkspaceInvitationsAsync()
    {
        var invitations = await _mediator.Send(new GetWorkspaceInvitationsQuery());
        return StatusCode(Common.Response.StatusCodes.OK, invitations);
    }

    [HttpPatch]
    public async Task<IActionResult> UpdateUserDetailsAsync([FromBody] UpdateUserDetailsCommand updateUserDetailsCommand)
    {
        await _mediator.Send(updateUserDetailsCommand);
        return StatusCode(Common.Response.StatusCodes.OK, "User details updated successfully.");
    }

    [HttpPatch("preferences")]
    public async Task<IActionResult> UpdateUserPreferencesAsync([FromBody] UpdateUserPreferencesCommand updateUserPreferencesCommand)
    {
        await _mediator.Send(updateUserPreferencesCommand);
        return StatusCode(Common.Response.StatusCodes.OK, "User preferences updated successfully.");
    }
}
