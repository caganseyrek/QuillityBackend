using MediatR;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Helpers;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Roles.Queries;

public class GetWorkspaceRolesQuery : IRequest<List<WorkspaceRoleDTO>>
{
    public Guid WorkspaceId { get; set; }
}

public class GetWorkspaceRolesQueryHandler : IRequestHandler<GetWorkspaceRolesQuery, List<WorkspaceRoleDTO>>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public GetWorkspaceRolesQueryHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<List<WorkspaceRoleDTO>> Handle(GetWorkspaceRolesQuery request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.None,
            cancellationToken);

        var workspaceRolesDTOs = _dbContext.WorkspaceRoles
            .Where(role => StringHelper.IsEqualIgnoreCase(role.WorkspaceId, workspace.Id))
            .Select(ObjectMapper.ToWorkspaceRoleDTO)
            .ToList();

        return workspaceRolesDTOs;
    }
}
