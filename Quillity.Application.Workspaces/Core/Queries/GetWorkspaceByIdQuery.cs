using MediatR;
using Quillity.Auth;
using Quillity.Common.Models;
using Quillity.Common.Values;

namespace Quillity.Application.Workspaces.Core.Queries;

public class GetWorkspaceByIdQuery : IRequest<WorkspaceModel>
{
    public Guid WorkspaceId { get; set; }
}

public class GetWorkspaceByIdQueryHandler : IRequestHandler<GetWorkspaceByIdQuery, WorkspaceModel?>
{
    private readonly PermissionService _workspaceService;

    public GetWorkspaceByIdQueryHandler(PermissionService workspaceService)
    {
        _workspaceService = workspaceService;
    }

    public async Task<WorkspaceModel?> Handle(GetWorkspaceByIdQuery request, CancellationToken cancellationToken)
    {
        var workspace = await _workspaceService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.None,
            cancellationToken);

        return workspace;
    }
}
