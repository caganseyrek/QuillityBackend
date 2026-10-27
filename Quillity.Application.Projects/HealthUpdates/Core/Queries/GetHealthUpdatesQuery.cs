using MediatR;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Helpers;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.HealthUpdates.Core.Queries;

public class GetHealthUpdatesQuery : IRequest<List<HealthUpdateDTO>>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid HealthUpdateId { get; set; }
}

public class GetHealthUpdatesQueryHandler : IRequestHandler<GetHealthUpdatesQuery, List<HealthUpdateDTO>>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public GetHealthUpdatesQueryHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<List<HealthUpdateDTO>> Handle(GetHealthUpdatesQuery request, CancellationToken cancellationToken)
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

        var healthUpdates = _dbContext.HealthUpdates
            .Where(update =>
                StringHelper.IsEqualIgnoreCase(update.ProjectId, project.Id) &&
                StringHelper.IsEqualIgnoreCase(update.Id, request.HealthUpdateId))
            .Select(ObjectMapper.ToHealthUpdateDTO)
            .ToList();

        return healthUpdates;
    }
}
