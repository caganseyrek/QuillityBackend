using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Models;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Estimates.Commands;

public class CreateEstimateItemCommandBody
{
    public required string Label { get; set; }
    public int MagnitudeIndex { get; set; }
} 

public class CreateEstimateItemCommand : CreateEstimateItemCommandBody, IRequest<EstimateItemDTO>
{
    public Guid WorkspaceId { get; set; }
    public Guid EstimateId { get; set; }
}

public class CreateEstimateItemCommandHandler : IRequestHandler<CreateEstimateItemCommand, EstimateItemDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public CreateEstimateItemCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<EstimateItemDTO> Handle(CreateEstimateItemCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.CreateEstimate,
            cancellationToken);

        var estimate = await _dbContext.Estimates
            .FirstOrDefaultAsync(estimate =>
                StringHelper.IsEqualIgnoreCase(estimate.Id, request.EstimateId) &&
                StringHelper.IsEqualIgnoreCase(estimate.WorkspaceId, workspace.Id), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.EstimateItem));

        var estimateItemWithSameLabel = await _dbContext.EstimateItems
            .AnyAsync(item =>
                StringHelper.IsEqualIgnoreCase(item.Label, request.Label) &&
                !StringHelper.IsEqualIgnoreCase(item.EstimateId, estimate.Id), cancellationToken);
        if (estimateItemWithSameLabel)
            throw new QuillityException(
                StatusCodes.BAD_REQUEST,
                ErrorMessages.GetEntryWithSamePropExistsError(ErrorMessageModel.EstimateItem, "name" , ErrorMessageScope.Workspace));

        var estimateItemWithSameMagnitude = await _dbContext.EstimateItems
            .AnyAsync(item =>
                item.MagnitudeIndex == request.MagnitudeIndex &&
                !StringHelper.IsEqualIgnoreCase(item.EstimateId, estimate.Id), cancellationToken);
        if (estimateItemWithSameMagnitude)
            throw new QuillityException(
                StatusCodes.BAD_REQUEST,
                ErrorMessages.GetEntryWithSamePropExistsError(ErrorMessageModel.EstimateItem, "magnitude", ErrorMessageScope.Workspace));

        var newEstimateItem = new EstimateItemModel
        {
            EstimateId = estimate.Id,
            Estimate = estimate,
            Label = request.Label,
            MagnitudeIndex = request.MagnitudeIndex,
        };

        await _dbContext.EstimateItems.AddAsync(newEstimateItem, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToEstimateItemDTO(newEstimateItem);
    }
}
