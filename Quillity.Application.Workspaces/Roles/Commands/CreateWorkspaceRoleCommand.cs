using MediatR;
using Quillity.Infrastructure;
using Quillity.Auth;
using Microsoft.EntityFrameworkCore;
using Quillity.Common.Helpers;
using Quillity.Common.Exceptions;
using Quillity.Common.Response;
using Quillity.Common.Models;
using Quillity.Common.Values;
using Quillity.Application.DTOs;

namespace Quillity.Application.Workspaces.Roles.Commands;

public class CreateWorkspaceRoleCommandBody
{
    public required string Name { get; set; }
    public int HierarchyIndex { get; set; }
    public WorkspacePermissions Permissions { get; set; }
}

public class CreateWorkspaceRoleCommand : CreateWorkspaceRoleCommandBody, IRequest<WorkspaceRoleDTO>
{
    public Guid WorkspaceId { get; set; }
}

public class CreateWorkspaceRoleCommandHandler : IRequestHandler<CreateWorkspaceRoleCommand, WorkspaceRoleDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public CreateWorkspaceRoleCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<WorkspaceRoleDTO> Handle(CreateWorkspaceRoleCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.CreateWorkspaceRole,
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

        // This is validated in the permission service so it is practically never null
        var currentUserMember = workspace.Members.SingleOrDefault()
            ?? throw new Exception(ErrorMessages.GetWorkspaceAccessLogicError());

        var currentUserRole = currentUserMember.Role;

        // A user should not be able to assign permissions that they do not have to the new role
        var canAssignPermissionsToRole = (request.Permissions & currentUserRole.Permissions) == request.Permissions;
        if (!canAssignPermissionsToRole)
            throw new QuillityException(StatusCodes.FORBIDDEN, "You cannot assign permissions you don’t have to a workspace role.");

        if (currentUserRole.HierarchyIndex > request.HierarchyIndex)
            throw new QuillityException(StatusCodes.FORBIDDEN, "You cannot create a workspace role higher in hierarchy than your current role.");

        if ((request.Permissions & ~WorkspacePermissions.AllValidWorkspacePermissions) != 0)
            throw new QuillityException(StatusCodes.BAD_REQUEST, "Role permissions are not valid");

        var newWorkspaceRole = new WorkspaceRoleModel
        {
            WorkspaceId = workspace.Id,
            Workspace = workspace,
            Name = request.Name,
            HierarchyIndex = request.HierarchyIndex,
            Permissions = request.Permissions,
        };

        await _dbContext.WorkspaceRoles.AddAsync(newWorkspaceRole, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToWorkspaceRoleDTO(newWorkspaceRole);
    }
}
