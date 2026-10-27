using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Estimates.Queries;

public class GetEstimateByIdQuery : IRequest<EstimateDTO>
{
    public Guid WorkspaceId { get; set; }
    public Guid EstimateId { get; set; }
}

public class GetEstimateByIdQueryHandler : IRequestHandler<GetEstimateByIdQuery, EstimateDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public GetEstimateByIdQueryHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<EstimateDTO> Handle(GetEstimateByIdQuery request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.None,
            cancellationToken);

        var existingEstimate = await _dbContext.Estimates
            .Include(estimate => estimate.Items)
            .FirstOrDefaultAsync(estimate =>
                StringHelper.IsEqualIgnoreCase(estimate.Id, request.EstimateId) &&
                StringHelper.IsEqualIgnoreCase(estimate.WorkspaceId, request.WorkspaceId), cancellationToken)
            ?? throw new QuillityException(StatusCodes.NOT_FOUND, ErrorMessages.GetNotFoundError(ErrorMessageModel.Estimate));

        return ObjectMapper.ToEstimateDTO(existingEstimate);
    }
}
