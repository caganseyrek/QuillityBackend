using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Estimates.Commands;

public class UpdateEstimateCommandBody
{
    public required string Name { get; set; }
}

public class UpdateEstimateCommand : CreateEstimateCommandBody, IRequest<EstimateDTO>
{
    public Guid WorkspaceId { get; set; }
    public Guid EstimateId { get; set; }
}

public class UpdateEstimateCommandHandler : IRequestHandler<UpdateEstimateCommand, EstimateDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public UpdateEstimateCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<EstimateDTO> Handle(UpdateEstimateCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.UpdateEstimate,
            cancellationToken);

        var existingEstimate = await _dbContext.Estimates
            .FirstOrDefaultAsync(estimate =>
                StringHelper.IsEqualIgnoreCase(estimate.WorkspaceId, workspace.Id) &&
                StringHelper.IsEqualIgnoreCase(estimate.Id, request.EstimateId), cancellationToken)
            ?? throw new QuillityException(StatusCodes.NOT_FOUND, ErrorMessages.GetNotFoundError(ErrorMessageModel.Estimate));

        var estimateWithSameName = await _dbContext.Estimates
            .AnyAsync(estimate =>
                !StringHelper.IsEqualIgnoreCase(estimate.Id, existingEstimate.Id) &&
                StringHelper.IsEqualIgnoreCase(estimate.WorkspaceId, workspace.Id) &&
                StringHelper.IsEqualIgnoreCase(estimate.Name, request.Name), cancellationToken);
        if (estimateWithSameName)
            throw new QuillityException(
                StatusCodes.BAD_REQUEST,
                ErrorMessages.GetEntryWithSamePropExistsError(ErrorMessageModel.Estimate, "name", ErrorMessageScope.Workspace));

        existingEstimate.Name = request.Name;

        _dbContext.Estimates.Update(existingEstimate);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToEstimateDTO(existingEstimate);
    }
}
