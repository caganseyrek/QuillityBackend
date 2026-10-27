using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Auth;
using Quillity.Common.Context;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.HealthUpdates.Core.Commands;

public class RemoveHealthUpdateReactionCommand : IRequest<Unit>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid HealthUpdateId { get; set; }
    public Guid CommentId { get; set; }
    public Guid ReactionId { get; set; }
}

public class RemoveHealthUpdateReactionCommandHandler : IRequestHandler<RemoveHealthUpdateReactionCommand, Unit>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;
    private readonly UserContext _userContext;

    public RemoveHealthUpdateReactionCommandHandler(
        QuillityDbContext dbContext,
        PermissionService permissionService,
        UserContext userContext)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
        _userContext = userContext;
    }

    public async Task<Unit> Handle(RemoveHealthUpdateReactionCommand request, CancellationToken cancellationToken)
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

        var existingReaction = await _dbContext.HealthUpdateReactions
            .FirstOrDefaultAsync(reaction =>
                StringHelper.IsEqualIgnoreCase(reaction.Id, request.ReactionId) &&
                StringHelper.IsEqualIgnoreCase(reaction.HealthUpdateId, request.HealthUpdateId), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.HealthUpdateReaction));

        var userProjectMembership = project.Members
            .FirstOrDefault(member => StringHelper.IsEqualIgnoreCase(member.User.Id, _userContext.User.Id))
            ?? throw new Exception(ErrorMessages.GetProjectAccessLogicError());

        if (!userProjectMembership.Role.Permissions.HasFlag(ProjectPermissions.DeleteAnyReaction) ||
            !StringHelper.IsEqualIgnoreCase(existingReaction.Author.Id, _userContext.User.Id))
            throw new QuillityException(
                StatusCodes.FORBIDDEN,
                "You are not permitted to delete this reaction.");

        _dbContext.HealthUpdateReactions.Remove(existingReaction);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

