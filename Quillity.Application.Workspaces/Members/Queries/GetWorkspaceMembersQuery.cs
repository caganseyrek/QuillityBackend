using MediatR;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Members.Queries;

public class GetWorkspaceMembersQuery : IRequest<List<WorkspaceMemberDTO>>
{
    public Guid WorkspaceId { get; set; }
}

public class GetWorkspaceMembersQueryHandler : IRequestHandler<GetWorkspaceMembersQuery, List<WorkspaceMemberDTO>>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public GetWorkspaceMembersQueryHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<List<WorkspaceMemberDTO>> Handle(GetWorkspaceMembersQuery request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.ViewWorkspaceInvitations,
            cancellationToken);

        var workspaceMembersDTOs = workspace.Members
            .Select(ObjectMapper.ToWorkspaceMemberDTO)
            .ToList();

        return workspaceMembersDTOs;
    }
}
