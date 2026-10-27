using MediatR;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Helpers;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Priorities.Queries;

public class GetPrioritiesQuery : IRequest<List<PriorityDTO>>
{
    public Guid WorkspaceId { get; set; }
}

public class GetPrioritiesQueryHandler : IRequestHandler<GetPrioritiesQuery, List<PriorityDTO>>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public GetPrioritiesQueryHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<List<PriorityDTO>> Handle(GetPrioritiesQuery request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.None,
            cancellationToken);

        var priorities = _dbContext.Priorities
            .Where(priority => StringHelper.IsEqualIgnoreCase(priority.WorkspaceId, workspace.Id))
            .Select(ObjectMapper.ToPriorityDTO)
            .ToList();

        return priorities;
    }
}
