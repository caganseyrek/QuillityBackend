using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Members.Commands;

public class RemoveWorkspaceMemberCommand : IRequest<Unit>
{
    public Guid UserId { get; set; }
    public Guid WorkspaceId { get; set; }
}

public class RemoveWorkspaceMemberCommandHandler : IRequestHandler<RemoveWorkspaceMemberCommand, Unit>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public RemoveWorkspaceMemberCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<Unit> Handle(RemoveWorkspaceMemberCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.RemoveWorkspaceMember,
            cancellationToken);

        var targetUserMembership = await _dbContext.WorkspaceMembers.
            FirstOrDefaultAsync(member =>
                StringHelper.IsEqualIgnoreCase(member.WorkspaceId, workspace.Id) &&
                StringHelper.IsEqualIgnoreCase(member.UserId, request.UserId), cancellationToken)
            ?? throw new QuillityException(StatusCodes.NOT_FOUND, "Specified user is not a member of this workspace");

        var canActOnTargetUser = await _permissionService.CanActOnUserInWorkspaceAsync(
            workspaceId: workspace.Id,
            targetUserId: request.UserId,
            requiredPermission: WorkspacePermissions.RemoveWorkspaceMember,
            cancellationToken);
        if (!canActOnTargetUser)
            throw new QuillityException(
                StatusCodes.FORBIDDEN,
                "You do not have permission to remove this user from the workspace.");

        _dbContext.WorkspaceMembers.Remove(targetUserMembership);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
