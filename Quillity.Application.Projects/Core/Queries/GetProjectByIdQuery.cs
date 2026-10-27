using MediatR;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Values;

namespace Quillity.Application.Projects.Core.Queries;

public class GetProjectByIdQuery : IRequest<ProjectDTO>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
}

public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectDTO>
{
    private readonly PermissionService _permissionService;

    public GetProjectByIdQueryHandler(PermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    public async Task<ProjectDTO> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
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

        return ObjectMapper.ToProjectDTO(project);
    }
}
