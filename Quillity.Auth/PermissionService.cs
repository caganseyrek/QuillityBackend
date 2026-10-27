using Microsoft.EntityFrameworkCore;
using Quillity.Common.Context;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Models;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Auth;

public class PermissionService
{
    private readonly QuillityDbContext _dbContext;
    private readonly UserContext _userContext;

    public PermissionService(QuillityDbContext dbContext, UserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<WorkspaceModel> AuthorizeWorkspaceAccessAsync(
        Guid workspaceId,
        WorkspacePermissions requiredPermission,
        CancellationToken cancellationToken = default)
    {
        var workspace = await _dbContext.Workspaces
            .Include(workspace => workspace.Members)
                .ThenInclude(member => member.Role)
            .Where(workspace => 
                StringHelper.IsEqualIgnoreCase(workspace.Id, workspaceId) &&
                workspace.Members.Any(member => StringHelper.IsEqualIgnoreCase(member.UserId, _userContext.User.Id)))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.Workspace));

        var member = workspace.Members
            .FirstOrDefault(member => StringHelper.IsEqualIgnoreCase(member.Id, _userContext.User.Id))
            ?? throw new QuillityException(
                StatusCodes.FORBIDDEN,
                "You are not a member of this workspace.");

        if (!member.Role.Permissions.HasFlag(requiredPermission))
            throw new QuillityException(
                StatusCodes.FORBIDDEN,
                ErrorMessages.GetForbiddenError(ErrorMessageScope.Workspace));

        return workspace;
    }

    public async Task<ProjectModel> AuthorizeProjectAccessAsync(
        Guid workspaceId,
        Guid projectId,
        ProjectPermissions requiredPermission,
        CancellationToken cancellationToken = default)
    {
        var project = await _dbContext.Projects
            .Include(project => project.Members)
                .ThenInclude(member => member.Role)
            .Where(project =>
                StringHelper.IsEqualIgnoreCase(project.WorkspaceId, workspaceId) &&
                StringHelper.IsEqualIgnoreCase(project.Id, projectId) &&
                project.Members.Any(member => StringHelper.IsEqualIgnoreCase(member.UserId, _userContext.User.Id)))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.Workspace));

        var member = project.Members
            .FirstOrDefault(member => StringHelper.IsEqualIgnoreCase(member.Id, _userContext.User.Id))
            ?? throw new QuillityException(
                StatusCodes.FORBIDDEN,
                "You are not a contributor of this project.");

        if (!member.Role.Permissions.HasFlag(requiredPermission))
            throw new QuillityException(
                StatusCodes.FORBIDDEN,
                ErrorMessages.GetForbiddenError(ErrorMessageScope.Project));

        return project;
    }

    public async Task<bool> CanActOnUserInWorkspaceAsync(
        Guid workspaceId,
        Guid targetUserId,
        WorkspacePermissions requiredPermission,
        CancellationToken cancellationToken = default)
    {
        var actor = await _dbContext.WorkspaceMembers
            .Include(member => member.Role)
            .FirstOrDefaultAsync(member =>
                StringHelper.IsEqualIgnoreCase(member.UserId, _userContext.User.Id) &&
                StringHelper.IsEqualIgnoreCase(member.WorkspaceId, workspaceId), cancellationToken);

        var target = await _dbContext.WorkspaceMembers
            .Include(member => member.Role)
            .FirstOrDefaultAsync(member =>
                StringHelper.IsEqualIgnoreCase(member.UserId, targetUserId) &&
                StringHelper.IsEqualIgnoreCase(member.WorkspaceId, workspaceId), cancellationToken);

        if (actor is null || target is null)
            return false;

        if (!actor.Role.Permissions.HasFlag(requiredPermission))
            return false;

        return actor.Role.HierarchyIndex < target.Role.HierarchyIndex; // Lower hierarchy index means higher authority
    }

    public async Task<bool> CanActOnUserInProjectAsync(
        Guid projectId,
        Guid targetUserId,
        ProjectPermissions requiredPermission,
        CancellationToken cancellationToken = default)
    {
        var actor = await _dbContext.ProjectMembers
            .Include(member => member.Role)
            .FirstOrDefaultAsync(member =>
                StringHelper.IsEqualIgnoreCase(member.UserId, _userContext.User.Id) &&
                StringHelper.IsEqualIgnoreCase(member.ProjectId, projectId), cancellationToken);

        var target = await _dbContext.ProjectMembers
            .Include(member => member.Role)
            .FirstOrDefaultAsync(member =>
                StringHelper.IsEqualIgnoreCase(member.UserId, targetUserId) &&
                StringHelper.IsEqualIgnoreCase(member.ProjectId, projectId), cancellationToken);

        if (actor is null || target is null)
            return false;

        if (!actor.Role.Permissions.HasFlag(requiredPermission))
            return false;

        return actor.Role.HierarchyIndex < target.Role.HierarchyIndex; // Lower hierarchy index means higher authority
    }
}
