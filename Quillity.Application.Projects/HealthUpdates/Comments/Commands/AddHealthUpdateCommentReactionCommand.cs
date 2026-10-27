using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Context;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Models;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.HealthUpdates.Comments.Commands;

public class AddHealthUpdateCommentReactionCommandBody
{
    public required string Emoji { get; set; }
}

public class AddHealthUpdateCommentReactionCommand : AddHealthUpdateCommentReactionCommandBody, IRequest<ReactionDTOBase>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid HealthUpdateId { get; set; }
    public Guid CommentId { get; set; }
}

public class AddHealthUpdateCommentReactionCommandHandler : IRequestHandler<AddHealthUpdateCommentReactionCommand, ReactionDTOBase>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;
    private readonly UserContext _userContext;

    public AddHealthUpdateCommentReactionCommandHandler(
        QuillityDbContext dbContext,
        PermissionService permissionService,
        UserContext userContext)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
        _userContext = userContext;
    }

    public async Task<ReactionDTOBase> Handle(AddHealthUpdateCommentReactionCommand request, CancellationToken cancellationToken)
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

        var projectMember = project.Members
            .FirstOrDefault(member => StringHelper.IsEqualIgnoreCase(member.Id, _userContext.User.Id))
            ?? throw new Exception(ErrorMessages.GetProjectAccessLogicError());

        var newHealthUpdateCommentReaction = new HealthUpdateCommentReactionModel
        {
            HealthUpdateCommentId = existingComment.Id,
            HealthUpdateComment = existingComment,
            Author = projectMember,
            AuthorId = projectMember.Id,
            Emoji = request.Emoji,
        };

        await _dbContext.HealthUpdateCommentReactions.AddAsync(newHealthUpdateCommentReaction, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToHealthUpdateCommentReactionDTO(newHealthUpdateCommentReaction);
    }
}
