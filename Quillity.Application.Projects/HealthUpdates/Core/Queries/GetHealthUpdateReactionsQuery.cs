using MediatR;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Helpers;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.HealthUpdates.Core.Queries;

public class GetHealthUpdateReactionsQuery : IRequest<List<HealthUpdateReactionDTO>>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid HealthUpdateId { get; set; }
}

public class GetHealthUpdateReactionsQueryHandler : IRequestHandler<GetHealthUpdateReactionsQuery, List<HealthUpdateReactionDTO>>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public GetHealthUpdateReactionsQueryHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<List<HealthUpdateReactionDTO>> Handle(GetHealthUpdateReactionsQuery request, CancellationToken cancellationToken)
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

        var reactions = _dbContext.HealthUpdateReactions
            .Where(reaction =>
                StringHelper.IsEqualIgnoreCase(reaction.HealthUpdateId, request.HealthUpdateId))
            .Select(ObjectMapper.ToHealthUpdateReactionDTO)
            .ToList();

        return reactions;
    }
}
