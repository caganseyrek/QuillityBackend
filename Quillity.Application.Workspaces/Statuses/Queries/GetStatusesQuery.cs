using MediatR;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Helpers;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Statuses.Queries;

public class GetStatusesQuery : IRequest<List<StatusDTO>>
{
    public Guid WorkspaceId { get; set; }
}

public class GetStatusesQueryHandler : IRequestHandler<GetStatusesQuery, List<StatusDTO>>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public GetStatusesQueryHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<List<StatusDTO>> Handle(GetStatusesQuery request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.None,
            cancellationToken);

        var statuses = _dbContext.Statuses
            .Where(status => StringHelper.IsEqualIgnoreCase(status.WorkspaceId, workspace.Id))
            .OrderBy(status => status.Group)
            .ThenBy(status => status.Name)
            .Select(ObjectMapper.ToStatusDTO)
            .ToList();

        return statuses;
    }
}
