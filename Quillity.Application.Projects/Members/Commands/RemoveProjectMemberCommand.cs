using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.Members.Commands;

public class RemoveProjectMemberCommand : IRequest<Unit>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid MemberId { get; set; }
}

public class RemoveProjectMemberCommandHandler : IRequestHandler<RemoveProjectMemberCommand, Unit>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public RemoveProjectMemberCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<Unit> Handle(RemoveProjectMemberCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.None,
            cancellationToken);

        var project = await _permissionService.AuthorizeProjectAccessAsync(
            workspaceId: workspace.Id,
            projectId: request.ProjectId,
            requiredPermission: ProjectPermissions.RemoveProjectMember,
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
            throw new QuillityException(StatusCodes.FORBIDDEN, "You remove this user from the project");

        _dbContext.ProjectMembers.Remove(targetUserMembership);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
