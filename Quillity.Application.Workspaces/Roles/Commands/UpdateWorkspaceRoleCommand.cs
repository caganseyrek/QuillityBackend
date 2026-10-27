using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Roles.Commands;

public class UpdateWorkspaceRoleCommandBody
{
    public required string Name { get; set; }
    public int HierarchyIndex { get; set; }
    public WorkspacePermissions Permissions { get; set; }
}

public class UpdateWorkspaceRoleCommand : UpdateWorkspaceRoleCommandBody, IRequest<WorkspaceRoleDTO>
{
    public Guid WorkspaceId { get; set; }
    public Guid RoleId { get; set; }
}

public class UpdateWorkspaceRoleCommandHandler : IRequestHandler<UpdateWorkspaceRoleCommand, WorkspaceRoleDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public UpdateWorkspaceRoleCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<WorkspaceRoleDTO> Handle(UpdateWorkspaceRoleCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.UpdateWorkspaceRole,
            cancellationToken);

        var roleWithSameName = await _dbContext.WorkspaceRoles
            .AnyAsync(role =>
                StringHelper.IsEqualIgnoreCase(role.WorkspaceId, workspace.Id) &&
                StringHelper.IsEqualIgnoreCase(role.Name, request.Name), cancellationToken);
        if (roleWithSameName)
            throw new QuillityException(
                StatusCodes.BAD_REQUEST,
                ErrorMessages.GetEntryWithSamePropExistsError(ErrorMessageModel.WorkspaceRole, "name", ErrorMessageScope.Workspace));

        var roleWithSameHierarchyIndex = await _dbContext.WorkspaceRoles
            .AnyAsync(role =>
                StringHelper.IsEqualIgnoreCase(role.WorkspaceId, workspace.Id) &&
                role.HierarchyIndex == request.HierarchyIndex, cancellationToken);
        if (roleWithSameHierarchyIndex)
            throw new QuillityException(
                StatusCodes.BAD_REQUEST,
                ErrorMessages.GetEntryWithSamePropExistsError(ErrorMessageModel.WorkspaceRole, "hierarchy index", ErrorMessageScope.Workspace));

        var existingRole = await _dbContext.WorkspaceRoles
            .FirstOrDefaultAsync(role =>
                StringHelper.IsEqualIgnoreCase(role.Id, request.RoleId) &&
                StringHelper.IsEqualIgnoreCase(role.WorkspaceId, workspace.Id), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.WorkspaceRole));

        // This is validated in the permission service so it is practically never null
        var currentUserMember = workspace.Members.SingleOrDefault()
            ?? throw new Exception(ErrorMessages.GetWorkspaceAccessLogicError());

        var currentUserRole = currentUserMember.Role;
        if (currentUserRole.HierarchyIndex > existingRole.HierarchyIndex)
            throw new QuillityException(StatusCodes.FORBIDDEN, "You cannot update a workspace role that is higher in hierarchy than your current role.");

        // A user should not be able to assign permissions that they do not have to the new role
        var canAssignPermissionsToRole = (request.Permissions & currentUserRole.Permissions) == request.Permissions;
        if (!canAssignPermissionsToRole)
            throw new QuillityException(StatusCodes.FORBIDDEN, "You cannot assign permissions you don’t have to a workspace role.");

        if (currentUserRole.HierarchyIndex > request.HierarchyIndex)
            throw new QuillityException(StatusCodes.FORBIDDEN, "You cannot update a workspace role to be higher in hierarchy than your current role.");

        existingRole.Name = request.Name;
        existingRole.HierarchyIndex = request.HierarchyIndex;
        existingRole.Permissions = request.Permissions;

        _dbContext.WorkspaceRoles.Update(existingRole);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToWorkspaceRoleDTO(existingRole);
    }
}
