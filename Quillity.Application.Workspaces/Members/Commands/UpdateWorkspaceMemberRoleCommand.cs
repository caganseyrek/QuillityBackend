using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Members.Commands;

public class UpdateWorkspaceMemberRoleCommandBody
{
    public Guid NewRoleId { get; set; }
}

public class UpdateWorkspaceMemberRoleCommand : UpdateWorkspaceMemberRoleCommandBody, IRequest<WorkspaceMemberDTO>
{
    public Guid WorkspaceId { get; set; }
    public Guid UserId { get; set; }
}

public class UpdateWorkspaceMemberRoleCommandHandler : IRequestHandler<UpdateWorkspaceMemberRoleCommand, WorkspaceMemberDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public UpdateWorkspaceMemberRoleCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<WorkspaceMemberDTO> Handle(UpdateWorkspaceMemberRoleCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.ChangeWorkspaceMemberRole,
            cancellationToken);

        var canActOnUser = await _permissionService.CanActOnUserInWorkspaceAsync(
            workspaceId: workspace.Id,
            targetUserId: request.UserId,
            requiredPermission: WorkspacePermissions.ChangeWorkspaceMemberRole,
            cancellationToken);
        if (!canActOnUser)
            throw new QuillityException(StatusCodes.FORBIDDEN, "You cannot update this user's workspace role");

        var existingRole = await _dbContext.WorkspaceRoles
            .FirstOrDefaultAsync(role =>
                StringHelper.IsEqualIgnoreCase(role.Id, request.NewRoleId) &&
                StringHelper.IsEqualIgnoreCase(role.WorkspaceId, workspace.Id), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.WorkspaceRole));

        var targetUserMembership = await _dbContext.WorkspaceMembers
            .FirstOrDefaultAsync(user =>
                StringHelper.IsEqualIgnoreCase(user.Id, request.UserId) &&
                StringHelper.IsEqualIgnoreCase(user.WorkspaceId, workspace.Id), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.WorkspaceMember));

        targetUserMembership.Role = existingRole;
        targetUserMembership.RoleId = existingRole.Id;

        _dbContext.WorkspaceMembers.Update(targetUserMembership);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToWorkspaceMemberDTO(targetUserMembership);
    }
}
