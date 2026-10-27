using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.Members.Commands;

public class ChangeProjectMemberRoleCommandBody
{
    public Guid NewRoleId { get; set; }
}

public class ChangeProjectMemberRoleCommand : ChangeProjectMemberRoleCommandBody, IRequest<ProjectMemberDTO>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid MemberId { get; set; }
}

public class ChangeProjectMemberRoleCommandHandler : IRequestHandler<ChangeProjectMemberRoleCommand, ProjectMemberDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public ChangeProjectMemberRoleCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<ProjectMemberDTO> Handle(ChangeProjectMemberRoleCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.None,
            cancellationToken);

        var project = await _permissionService.AuthorizeProjectAccessAsync(
            workspaceId: workspace.Id,
            projectId: request.ProjectId,
            requiredPermission: ProjectPermissions.ChangeProjectMemberRole,
            cancellationToken);

        var targetUserMembership = await _dbContext.ProjectMembers
            .FirstOrDefaultAsync(user =>
                StringHelper.IsEqualIgnoreCase(user.Id, request.MemberId) &&
                StringHelper.IsEqualIgnoreCase(user.ProjectId, workspace.Id), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.ProjectMember));

        var canActOnUserInWorkspace = await _permissionService.CanActOnUserInWorkspaceAsync(
            workspaceId: workspace.Id,
            targetUserId: targetUserMembership.UserId,
            requiredPermission: WorkspacePermissions.ChangeWorkspaceMemberRole,
            cancellationToken);

        var canActOnUserInProject = await _permissionService.CanActOnUserInProjectAsync(
            projectId: project.Id,
            targetUserId: targetUserMembership.UserId,
            requiredPermission: ProjectPermissions.ChangeProjectMemberRole,
            cancellationToken);

        if (!canActOnUserInWorkspace || !canActOnUserInProject)
            throw new QuillityException(StatusCodes.FORBIDDEN, "You cannot update this user's project role");

        var existingRole = await _dbContext.ProjectRoles
            .FirstOrDefaultAsync(role =>
                StringHelper.IsEqualIgnoreCase(role.Id, request.NewRoleId) &&
                StringHelper.IsEqualIgnoreCase(role.ProjectId, workspace.Id), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.ProjectRole));

        targetUserMembership.Role = existingRole;
        targetUserMembership.RoleId = existingRole.Id;

        _dbContext.ProjectMembers.Update(targetUserMembership);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToProjectMemberDTO(targetUserMembership);
    }
}