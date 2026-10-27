using MediatR;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Helpers;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.HealthUpdates.Comments.Queries;

public class GetHealthUpdateCommentReactionsQuery : IRequest<List<HealthUpdateCommentReactionDTO>>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid HealthUpdateId { get; set; }
    public Guid CommentId { get; set; }
}

public class GetHealthUpdateCommentReactionsQueryHandler : IRequestHandler<GetHealthUpdateCommentReactionsQuery, List<HealthUpdateCommentReactionDTO>>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public GetHealthUpdateCommentReactionsQueryHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<List<HealthUpdateCommentReactionDTO>> Handle(GetHealthUpdateCommentReactionsQuery request, CancellationToken cancellationToken)
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

        var reactions = _dbContext.HealthUpdateCommentReactions
            .Where(reaction =>
                StringHelper.IsEqualIgnoreCase(reaction.HealthUpdateCommentId, request.CommentId))
            .Select(ObjectMapper.ToHealthUpdateCommentReactionDTO)
            .ToList();

        return reactions;
    }
}
