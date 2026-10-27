using MediatR;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Helpers;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Estimates.Queries;

public class GetEstimatesQuery : IRequest<List<EstimateDTO>>
{
    public Guid WorkspaceId { get; set; }
}

public class GetEstimatesQueryHandler : IRequestHandler<GetEstimatesQuery, List<EstimateDTO>>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public GetEstimatesQueryHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<List<EstimateDTO>> Handle(GetEstimatesQuery request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.None,
            cancellationToken);

        var estimates = _dbContext.Estimates
            .Where(estimate => StringHelper.IsEqualIgnoreCase(estimate.WorkspaceId, workspace.Id))
            .Select(ObjectMapper.ToEstimateDTO)
            .ToList();

        return estimates;
    }
}
