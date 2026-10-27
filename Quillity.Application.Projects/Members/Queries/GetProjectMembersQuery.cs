using MediatR;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Values;

namespace Quillity.Application.Projects.Members.Queries;

public class GetProjectMembersQuery : IRequest<List<ProjectMemberDTO>>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
}

public class GetProjectMembersQueryHandler : IRequestHandler<GetProjectMembersQuery, List<ProjectMemberDTO>>
{
    private readonly PermissionService _permissionService;

    public GetProjectMembersQueryHandler(PermissionService permissionService)
    {
        _permissionService = permissionService;
    }

    public async Task<List<ProjectMemberDTO>> Handle(GetProjectMembersQuery request, CancellationToken cancellationToken)
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

        var projectMembers = project.Members.Select(ObjectMapper.ToProjectMemberDTO).ToList();
        return projectMembers;
    }
}
