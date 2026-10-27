using MediatR;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Helpers;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.Iterations.Queries;

public class GetIterationsQuery : IRequest<List<IterationDTO>>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
}

internal class GetIterationsQueryHandler : IRequestHandler<GetIterationsQuery, List<IterationDTO>>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public GetIterationsQueryHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<List<IterationDTO>> Handle(GetIterationsQuery request, CancellationToken cancellationToken)
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

        var iterations = _dbContext.Iterations
            .Where(iteration => StringHelper.IsEqualIgnoreCase(iteration.ProjectId, project.Id))
            .Select(ObjectMapper.ToIterationDTO)
            .ToList();

        return iterations;
    }
}
