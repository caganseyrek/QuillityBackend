using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Auth;
using Quillity.Common.Context;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.HealthUpdates.Comments.Commands;

public class DeleteHealthUpdateCommentCommand : IRequest<Unit>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid HealthUpdateId { get; set; }
    public Guid CommentId { get; set; }
}

public class DeleteHealthUpdateCommentCommandHandler : IRequestHandler<DeleteHealthUpdateCommentCommand, Unit>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;
    private readonly UserContext _userContext;

    public DeleteHealthUpdateCommentCommandHandler(
        QuillityDbContext dbContext,
        PermissionService permissionService,
        UserContext userContext)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
        _userContext = userContext;
    }

    public async Task<Unit> Handle(DeleteHealthUpdateCommentCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.None,
            cancellationToken);

        var project = await _permissionService.AuthorizeProjectAccessAsync(
            workspaceId: workspace.Id,
            projectId: request.ProjectId,
            requiredPermission: ProjectPermissions.None,
            cancellationToken);

        var existingHealthUpdateComment = await _dbContext.HealthUpdateComments
            .FirstOrDefaultAsync(comment =>
                StringHelper.IsEqualIgnoreCase(comment.Id, request.CommentId) &&
                StringHelper.IsEqualIgnoreCase(comment.HealthUpdateId, request.HealthUpdateId), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.HealthUpdateComment));

        var userProjectMembership = project.Members
            .FirstOrDefault(member => StringHelper.IsEqualIgnoreCase(member.User.Id, _userContext.User.Id))
            ?? throw new Exception(ErrorMessages.GetProjectAccessLogicError());

        if (!userProjectMembership.Role.Permissions.HasFlag(ProjectPermissions.DeleteAnyComment) ||
            !StringHelper.IsEqualIgnoreCase(existingHealthUpdateComment.Author.Id, _userContext.User.Id))
            throw new QuillityException(
                StatusCodes.FORBIDDEN,
                "You are not permitted to delete this comment.");

        _dbContext.HealthUpdateComments.Remove(existingHealthUpdateComment);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
