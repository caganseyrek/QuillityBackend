using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.Roles.Command;

public class UpdateProjectRoleCommandBody
{
    public required string Name { get; set; }
    public int HierarchyIndex { get; set; }
    public ProjectPermissions Permissions { get; set; }
}

public class UpdateProjectRoleCommand : UpdateProjectRoleCommandBody, IRequest<ProjectRoleDTO>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid RoleId { get; set; }
}

public class UpdateProjectRoleCommandHandler : IRequestHandler<UpdateProjectRoleCommand, ProjectRoleDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public UpdateProjectRoleCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<ProjectRoleDTO> Handle(UpdateProjectRoleCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.None,
            cancellationToken);

        var project = await _permissionService.AuthorizeProjectAccessAsync(
            workspaceId: workspace.Id,
            projectId: request.ProjectId,
            requiredPermission: ProjectPermissions.UpdateProjectRole,
            cancellationToken);

        var roleWithSameName = await _dbContext.ProjectRoles
            .AnyAsync(role =>
                StringHelper.IsEqualIgnoreCase(role.ProjectId, project.Id) &&
                StringHelper.IsEqualIgnoreCase(role.Name, request.Name), cancellationToken);
        if (roleWithSameName)
            throw new QuillityException(
                StatusCodes.BAD_REQUEST,
                ErrorMessages.GetEntryWithSamePropExistsError(ErrorMessageModel.ProjectRole, "name", ErrorMessageScope.Project));

        var roleWithSameHierarchyIndex = await _dbContext.ProjectRoles
            .AnyAsync(role =>
                StringHelper.IsEqualIgnoreCase(role.ProjectId, project.Id) &&
                role.HierarchyIndex == request.HierarchyIndex, cancellationToken);
        if (roleWithSameHierarchyIndex)
            throw new QuillityException(
                StatusCodes.BAD_REQUEST,
                ErrorMessages.GetEntryWithSamePropExistsError(ErrorMessageModel.ProjectRole, "hierarchy index", ErrorMessageScope.Project));

        var existingRole = await _dbContext.ProjectRoles
            .FirstOrDefaultAsync(role =>
                StringHelper.IsEqualIgnoreCase(role.Id, request.RoleId) &&
                StringHelper.IsEqualIgnoreCase(role.ProjectId, project.Id), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.ProjectRole));

        // This is validated in the permission service so it is practically never null
        var currentUserMember = project.Members.SingleOrDefault()
            ?? throw new Exception(ErrorMessages.GetProjectAccessLogicError());

        var currentUserRole = currentUserMember.Role;
        if (currentUserRole.HierarchyIndex > existingRole.HierarchyIndex)
            throw new QuillityException(StatusCodes.FORBIDDEN, "You cannot update a project role that is higher in hierarchy than your current role.");

        // A user should not be able to assign permissions that they do not have to the new role
        var canAssignPermissionsToRole = (request.Permissions & currentUserRole.Permissions) == request.Permissions;
        if (!canAssignPermissionsToRole)
            throw new QuillityException(StatusCodes.FORBIDDEN, "You cannot assign permissions you don’t have to a project role.");

        if (currentUserRole.HierarchyIndex > request.HierarchyIndex)
            throw new QuillityException(StatusCodes.FORBIDDEN, "You cannot update a project role to be higher in hierarchy than your current role.");

        existingRole.Name = request.Name;
        existingRole.HierarchyIndex = request.HierarchyIndex;
        existingRole.Permissions = request.Permissions;

        _dbContext.ProjectRoles.Update(existingRole);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToProjectRoleDTO(existingRole);
    }
}
