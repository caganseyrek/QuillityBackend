using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.Roles.Command;

public class DeleteProjectRoleCommand : IRequest<Unit>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid RoleId { get; set; }
}

public class DeleteProjectRoleCommandHandler : IRequestHandler<DeleteProjectRoleCommand, Unit>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public DeleteProjectRoleCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<Unit> Handle(DeleteProjectRoleCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.None,
            cancellationToken);

        var project = await _permissionService.AuthorizeProjectAccessAsync(
            workspaceId: workspace.Id,
            projectId: request.ProjectId,
            requiredPermission: ProjectPermissions.DeleteProjectRole,
            cancellationToken);

        if (StringHelper.IsEqualIgnoreCase(project.LeadRoleId, request.RoleId))
            throw new QuillityException(StatusCodes.FORBIDDEN, "You cannot delete the project's lead role.");

        if (StringHelper.IsEqualIgnoreCase(project.DefaultRoleId, request.RoleId))
            throw new QuillityException(StatusCodes.FORBIDDEN, "You cannot delete the project's default role.");

        var existingRole = await _dbContext.ProjectRoles
            .FirstOrDefaultAsync(role =>
                StringHelper.IsEqualIgnoreCase(role.ProjectId, project.Id) &&
                StringHelper.IsEqualIgnoreCase(role.Id, request.RoleId), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.ProjectRole));

        // This is validated in the permission service so it is practically never null
        var currentUserMember = project.Members.SingleOrDefault()
            ?? throw new Exception(ErrorMessages.GetProjectAccessLogicError());

        var currentUserRole = currentUserMember.Role;
        if (currentUserRole.HierarchyIndex > existingRole.HierarchyIndex)
            throw new QuillityException(StatusCodes.FORBIDDEN, "You cannot delete a project role that is higher in hierarchy than your current role.");

        _dbContext.ProjectRoles.Remove(existingRole);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
