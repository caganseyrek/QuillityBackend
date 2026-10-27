using MediatR;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Helpers;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.HealthUpdates.Comments.Queries;

public class GetHealthUpdateCommentsQuery : IRequest<List<HealthUpdateCommentDTO>>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid HealthUpdateId { get; set; }
}

public class GetHealthUpdateCommentsQueryHandler : IRequestHandler<GetHealthUpdateCommentsQuery, List<HealthUpdateCommentDTO>>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public GetHealthUpdateCommentsQueryHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<List<HealthUpdateCommentDTO>> Handle(GetHealthUpdateCommentsQuery request, CancellationToken cancellationToken)
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

        var healthUpdateComments = _dbContext.HealthUpdateComments
            .Where(update =>
                StringHelper.IsEqualIgnoreCase(update.HealthUpdateId, request.HealthUpdateId) &&
                StringHelper.IsEqualIgnoreCase(update.Id, request.HealthUpdateId))
            .Select(ObjectMapper.ToHealthUpdateCommentDTO)
            .ToList();

        return healthUpdateComments;
    }
}
