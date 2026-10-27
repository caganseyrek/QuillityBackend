using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Common.Context;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Models;
using Quillity.Common.Response;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Members.Commands;

public class JoinWorkspaceCommand : IRequest<Unit>
{
    public Guid InvitedWorkspaceId { get; set; }
}

public class AcceptWorkspaceInvitationCommandHandler : IRequestHandler<JoinWorkspaceCommand, Unit>
{
    private readonly QuillityDbContext _dbContext;
    private readonly UserContext _userContext;

    public AcceptWorkspaceInvitationCommandHandler(QuillityDbContext dbContext, UserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<Unit> Handle(JoinWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var existingInvitation = await _dbContext.Invitations
            .FirstOrDefaultAsync(invitation =>
                StringHelper.IsEqualIgnoreCase(invitation.InvitedUserId, _userContext.User.Id) &&
                StringHelper.IsEqualIgnoreCase(invitation.InvitedWorkspaceId, request.InvitedWorkspaceId), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.BAD_REQUEST,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.Invitation));

        var isAlreadyMember = await _dbContext.WorkspaceMembers
            .AnyAsync(member =>
                StringHelper.IsEqualIgnoreCase(member.UserId, _userContext.User.Id) &&
                StringHelper.IsEqualIgnoreCase(member.WorkspaceId, existingInvitation.InvitedWorkspaceId), cancellationToken);
        if (isAlreadyMember)
            throw new QuillityException(StatusCodes.BAD_REQUEST, "You are already a member of this workspace");

        var invitedWorkspace = await _dbContext.Workspaces
            .FirstOrDefaultAsync(workspace =>
                StringHelper.IsEqualIgnoreCase(workspace.Id, existingInvitation.InvitedWorkspaceId), cancellationToken)
            ?? throw new QuillityException(StatusCodes.NOT_FOUND, ErrorMessages.GetNotFoundError(ErrorMessageModel.Invitation));

        existingInvitation.AcceptedAtMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var newMember = new WorkspaceMemberModel
        {
            Workspace = invitedWorkspace,
            WorkspaceId = invitedWorkspace.Id,
            User = _userContext.User,
            UserId = _userContext.User.Id,
            Role = invitedWorkspace.DefaultRole!,
            RoleId = invitedWorkspace.DefaultRoleId,
        };

        _dbContext.Invitations.Update(existingInvitation);
        await _dbContext.WorkspaceMembers.AddAsync(newMember, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
