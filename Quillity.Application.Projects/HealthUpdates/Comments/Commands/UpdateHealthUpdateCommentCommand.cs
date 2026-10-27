using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Context;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.HealthUpdates.Comments.Commands;

public class UpdateProjectHealthUpdateCommentCommandBody
{
    public Guid? ParentCommentId { get; set; } = null;
    public required string Content { get; set; }
    public bool IsThreadResolved { get; set; }
}

public class UpdateHealthUpdateCommentCommand : UpdateProjectHealthUpdateCommentCommandBody, IRequest<HealthUpdateCommentDTO>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid HealthUpdateId { get; set; }
    public Guid CommentId { get; set; }
}

public class UpdateProjectHealthUpdateCommentCommandHandler : IRequestHandler<UpdateHealthUpdateCommentCommand, HealthUpdateCommentDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;
    private readonly UserContext _userContext;

    public UpdateProjectHealthUpdateCommentCommandHandler(
        QuillityDbContext dbContext,
        PermissionService permissionService,
        UserContext userContext)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
        _userContext = userContext;
    }

    public async Task<HealthUpdateCommentDTO> Handle(UpdateHealthUpdateCommentCommand request, CancellationToken cancellationToken)
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

        var existingComment = await _dbContext.HealthUpdateComments
            .FirstOrDefaultAsync(comment =>
                StringHelper.IsEqualIgnoreCase(comment.Id, request.CommentId) &&
                StringHelper.IsEqualIgnoreCase(comment.HealthUpdateId, request.HealthUpdateId) &&
                StringHelper.IsEqualIgnoreCase(comment.Author.Id, _userContext.User.Id), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.HealthUpdateComment));

        if (StringHelper.IsEqualIgnoreCase(existingComment.Author.Id, _userContext.User.Id))
            throw new QuillityException(
                StatusCodes.FORBIDDEN,
                ErrorMessages.GetForbiddenError(ErrorMessageScope.Project));

        if (existingComment.ParentComment is null && string.IsNullOrWhiteSpace(request.ParentCommentId.ToString()))
            throw new QuillityException(
                StatusCodes.BAD_REQUEST,
                ErrorMessages.GetCannotBeEmptyError(ErrorMessageModel.HealthUpdateComment, "parent comment id"));

        existingComment.Content = request.Content;
        existingComment.IsThreadResolved = request.IsThreadResolved;
        if (existingComment.IsThreadResolved)
            existingComment.ThreadResolvedAtMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        _dbContext.HealthUpdateComments.Update(existingComment);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToHealthUpdateCommentDTO(existingComment);
    }
}