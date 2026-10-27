using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Roles.Commands;

public class DeleteWorkspaceRoleCommand : IRequest<Unit>
{
    public Guid WorkspaceId { get; set; }
    public Guid RoleId { get; set; }
}

public class DeleteWorkspaceRoleCommandHandler : IRequestHandler<DeleteWorkspaceRoleCommand, Unit>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public DeleteWorkspaceRoleCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<Unit> Handle(DeleteWorkspaceRoleCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.DeleteWorkspaceRole,
            cancellationToken);

        if (StringHelper.IsEqualIgnoreCase(workspace.OwnerRoleId, request.RoleId))
            throw new QuillityException(StatusCodes.FORBIDDEN, "You cannot delete the workspace's owner role.");

        if (StringHelper.IsEqualIgnoreCase(workspace.DefaultRoleId, request.RoleId))
            throw new QuillityException(StatusCodes.FORBIDDEN, "You cannot delete the workspace's default role.");

        var existingRole = await _dbContext.WorkspaceRoles
            .FirstOrDefaultAsync(role =>
                StringHelper.IsEqualIgnoreCase(role.WorkspaceId, workspace.Id) &&
                StringHelper.IsEqualIgnoreCase(role.Id, request.RoleId), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.WorkspaceRole));

        // This is validated in the permission service so it is practically never null
        var currentUserMember = workspace.Members.SingleOrDefault()
            ?? throw new Exception(ErrorMessages.GetWorkspaceAccessLogicError());

        var currentUserRole = currentUserMember.Role;
        if (currentUserRole.HierarchyIndex > existingRole.HierarchyIndex)
            throw new QuillityException(StatusCodes.FORBIDDEN, "You cannot delete a workspace role that is higher in hierarchy than your current role.");

        _dbContext.WorkspaceRoles.Remove(existingRole);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
