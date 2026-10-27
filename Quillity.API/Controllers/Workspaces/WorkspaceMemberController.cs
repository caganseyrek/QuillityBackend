using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quillity.Application.Workspaces.Members.Commands;
using Quillity.Application.Workspaces.Members.Queries;

namespace Quillity.API.Controllers.Workspaces;

[Authorize]
[ApiController]
[Route("workspaces/{workspaceId}")]
public class WorkspaceMemberController : ControllerBase
{
    private readonly IMediator _mediator;

    public WorkspaceMemberController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("invitations")]
    public async Task<IActionResult> GetWorkspaceInvitationsAsync(Guid workspaceId)
    {
        var invitations = await _mediator.Send(new GetWorkspaceInvitationsQuery
        {
            WorkspaceId = workspaceId,
        });
        return StatusCode(Common.Response.StatusCodes.OK, invitations);
    }

    [HttpGet("invitations/{invitationId}")]
    public async Task<IActionResult> GetWorkspaceInvitationsAsync(Guid workspaceId, Guid invitationId)
    {
        var invitation = await _mediator.Send(new GetWorkspaceInvitationByIdQuery
        {
            WorkspaceId = workspaceId,
            InvitationId = invitationId,
        });
        return StatusCode(Common.Response.StatusCodes.OK, invitation);
    }

    [HttpGet("members")]
    public async Task<IActionResult> GetWorkspaceMembersAsync(Guid workspaceId)
    {
        var members = await _mediator.Send(new GetWorkspaceMembersQuery
        {
            WorkspaceId = workspaceId,
        });
        return StatusCode(Common.Response.StatusCodes.OK, members);
    }

    [HttpPost("invitations")]
    public async Task<IActionResult> SendWorkspaceInvitationAsync(
    Guid workspaceId,
    [FromBody] SendWorkspaceInvitationCommandBody command)
    {
        await _mediator.Send(new SendWorkspaceInvitationCommand
        {
            WorkspaceId = workspaceId,
            InvitedUserEmail = command.InvitedUserEmail,
            InvitationNote = command.InvitationNote,
        });
        return StatusCode(Common.Response.StatusCodes.CREATED, "User invited to workspace successfully");
    }

    [HttpDelete("invitations/{invitationId}")]
    public async Task<IActionResult> CancelWorkspaceInvitationAsync(Guid workspaceId, Guid invitationId)
    {
        await _mediator.Send(new CancelWorkspaceInvitationCommand
        {
            WorkspaceId = workspaceId,
            InvitationId = invitationId,
        });
        return StatusCode(Common.Response.StatusCodes.OK, "Workspace invitation cancelled successfully");
    }

    [HttpPost("join")]
    public async Task<IActionResult> JoinWorkspaceAsync([FromBody] JoinWorkspaceCommand joinWorkspaceCommand)
    {
        await _mediator.Send(joinWorkspaceCommand);
        return StatusCode(Common.Response.StatusCodes.OK, "Joined workspace successfully");
    }

    [HttpPost("leave")]
    public async Task<IActionResult> LeaveWorkspaceAsync(Guid workspaceId)
    {
        await _mediator.Send(new LeaveWorkspaceCommand { WorkspaceId = workspaceId });
        return StatusCode(Common.Response.StatusCodes.OK, "Left workspace successfully");
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> TransferWorkspaceOwnershipAsync(
        Guid workspaceId,
        [FromBody] TransferWorkspaceOwnershipCommandBody transferWorkspaceOwnershipCommandBody)
    {
        await _mediator.Send(new TransferWorkspaceOwnershipCommand
        {
            WorkspaceId = workspaceId,
            NewOwnerId = transferWorkspaceOwnershipCommandBody.NewOwnerId,
        });
        return StatusCode(Common.Response.StatusCodes.OK, "Workspace ownership transferred successfully");
    }

    [HttpDelete("members/{memberId}")]
    public async Task<IActionResult> RemoveWorkspaceMemberAsync(Guid workspaceId, Guid memberId)
    {
        await _mediator.Send(new RemoveWorkspaceMemberCommand
        {
            WorkspaceId = workspaceId,
            UserId = memberId,
        });
        return StatusCode(Common.Response.StatusCodes.OK, "Member removed from workspace successfully");
    }

    [HttpPatch("members/{memberId}")]
    public async Task<IActionResult> UpdateWorkspaceMemberRoleAsync(
        Guid workspaceId,
        Guid memberId,
        [FromBody] UpdateWorkspaceMemberRoleCommandBody updateWorkspaceMemberRoleCommandBody)
    {
        await _mediator.Send(new UpdateWorkspaceMemberRoleCommand
        {
            WorkspaceId = workspaceId,
            UserId = memberId,
            NewRoleId = updateWorkspaceMemberRoleCommandBody.NewRoleId,
        });
        return StatusCode(Common.Response.StatusCodes.OK, "Member's role updated successfully");
    }
}
