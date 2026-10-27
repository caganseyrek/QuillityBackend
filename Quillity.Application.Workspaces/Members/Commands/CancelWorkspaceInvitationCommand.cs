using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Members.Commands;

public class CancelWorkspaceInvitationCommand : IRequest<Unit>
{
    public Guid InvitationId { get; set; }
    public Guid WorkspaceId { get; set; }
}

public class CancelWorkspaceInvitationCommandHandler : IRequestHandler<CancelWorkspaceInvitationCommand, Unit>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public CancelWorkspaceInvitationCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<Unit> Handle(CancelWorkspaceInvitationCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.CancelWorkspaceInvitation,
            cancellationToken);

        var existingInvitation = await _dbContext.Invitations
            .FirstOrDefaultAsync(invitation =>
                StringHelper.IsEqualIgnoreCase(invitation.Id, request.InvitationId) &&
                StringHelper.IsEqualIgnoreCase(invitation.InvitedWorkspace.Id, workspace.Id), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.Invitation));

        if (existingInvitation.AcceptedAtMs is not null)
            throw new QuillityException(StatusCodes.BAD_REQUEST, "User already accepted the invitation");

        var acceptedUserMember = await _dbContext.WorkspaceMembers
            .AnyAsync(member =>
                StringHelper.IsEqualIgnoreCase(member.WorkspaceId, workspace.Id) &&
                StringHelper.IsEqualIgnoreCase(member.UserId, existingInvitation.InvitedUserId), cancellationToken);
        if (acceptedUserMember)
            throw new QuillityException(StatusCodes.BAD_REQUEST, "User is already a member of the workspace");

        _dbContext.Invitations.Remove(existingInvitation);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
