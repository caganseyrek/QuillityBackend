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

public class CreateHealthUpdateCommentCommandBody
{
    public Guid? ParentCommentId { get; set; } = null;
    public required string Content { get; set; }
}

public class CreateHealthUpdateCommentCommand : CreateHealthUpdateCommentCommandBody, IRequest<HealthUpdateCommentDTO>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid HealthUpdateId { get; set; }
}

public class CreateHealthUpdateCommentCommandHandler : IRequestHandler<CreateHealthUpdateCommentCommand, HealthUpdateCommentDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;
    private readonly UserContext _userContext;

    public CreateHealthUpdateCommentCommandHandler(
        QuillityDbContext dbContext,
        PermissionService permissionService,
        UserContext userContext)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
        _userContext = userContext;
    }

    public async Task<HealthUpdateCommentDTO> Handle(CreateHealthUpdateCommentCommand request, CancellationToken cancellationToken)
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

        var existingHealthUpdate = await _dbContext.HealthUpdates
            .FirstOrDefaultAsync(update =>
                StringHelper.IsEqualIgnoreCase(update.ProjectId, project.Id) &&
                StringHelper.IsEqualIgnoreCase(update.Id, request.HealthUpdateId), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.HealthUpdateComment));

        var projectMember = project.Members
            .FirstOrDefault(member => StringHelper.IsEqualIgnoreCase(member.Id, _userContext.User.Id))
            ?? throw new Exception(ErrorMessages.GetProjectAccessLogicError());

        var newHealthUpdateComment = new HealthUpdateCommentModel
        {
            HealthUpdate = existingHealthUpdate,
            HealthUpdateId = existingHealthUpdate.Id,
            Author = projectMember,
            AuthorId = projectMember.Id,
            Content = request.Content,
            ParentComment = null,
            ParentCommentId = null,
        };

        if (request.ParentCommentId is not null)
        {
            var existingParentComment = await _dbContext.HealthUpdateComments
                .FirstOrDefaultAsync(comment =>
                    StringHelper.IsEqualIgnoreCase(comment.Id, request.ParentCommentId) &&
                    StringHelper.IsEqualIgnoreCase(comment.HealthUpdateId, existingHealthUpdate.Id), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.HealthUpdate));

            newHealthUpdateComment.ParentCommentId = existingParentComment.Id;
            newHealthUpdateComment.ParentComment = existingParentComment;
        }

        await _dbContext.HealthUpdateComments.AddAsync(newHealthUpdateComment, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToHealthUpdateCommentDTO(newHealthUpdateComment);
    }
}
