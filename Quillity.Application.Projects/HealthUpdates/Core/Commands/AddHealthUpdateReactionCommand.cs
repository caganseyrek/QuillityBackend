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

namespace Quillity.Application.Projects.HealthUpdates.Core.Commands;

public class AddProjectHealthUpdateReactionCommandBody
{
    public required string Emoji { get; set; }
}

public class AddProjectHealthUpdateCommentReactionCommand : AddProjectHealthUpdateReactionCommandBody, IRequest<ReactionDTOBase>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid HealthUpdateId { get; set; }
}

public class AddProjectHealthUpdateReactionCommandHandler : IRequestHandler<AddProjectHealthUpdateCommentReactionCommand, ReactionDTOBase>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;
    private readonly UserContext _userContext;

    public AddProjectHealthUpdateReactionCommandHandler(
        QuillityDbContext dbContext,
        PermissionService permissionService,
        UserContext userContext)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
        _userContext = userContext;
    }

    public async Task<ReactionDTOBase> Handle(AddProjectHealthUpdateCommentReactionCommand request, CancellationToken cancellationToken)
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

        var projectMember = project.Members
            .FirstOrDefault(member => StringHelper.IsEqualIgnoreCase(member.Id, _userContext.User.Id))
            ?? throw new Exception(ErrorMessages.GetProjectAccessLogicError());

        var existingHealthUpdate = await _dbContext.HealthUpdates
            .FirstOrDefaultAsync(comment =>
                StringHelper.IsEqualIgnoreCase(comment.Id, request.HealthUpdateId), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.HealthUpdate));
 
        var newHealthUpdateReaction = new HealthUpdateReactionModel
        {
            HealthUpdate = existingHealthUpdate,
            HealthUpdateId = existingHealthUpdate.Id,
            Author = projectMember,
            AuthorId = projectMember.Id,
            Emoji = request.Emoji,
        };

        await _dbContext.HealthUpdateReactions.AddAsync(newHealthUpdateReaction, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToHealthUpdateReactionDTO(newHealthUpdateReaction);
    }
}
