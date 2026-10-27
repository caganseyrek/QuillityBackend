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

public class UpdateEstimateItemCommandBody
{
    public required string Label { get; set; }
    public int MagnitudeIndex { get; set; }
}

public class UpdateEstimateItemCommand : UpdateEstimateItemCommandBody, IRequest<EstimateItemDTO>
{
    public Guid WorkspaceId { get; set; }
    public Guid EstimateId { get; set; }
    public Guid EstimateItemId { get; set; }
}

public class UpdateEstimateItemCommandHandler : IRequestHandler<UpdateEstimateItemCommand, EstimateItemDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public UpdateEstimateItemCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<EstimateItemDTO> Handle(UpdateEstimateItemCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.UpdateEstimate,
            cancellationToken);

        var existingEstimateItem = await _dbContext.EstimateItems
            .FirstOrDefaultAsync(item =>
                StringHelper.IsEqualIgnoreCase(item.Id, request.EstimateItemId) &&
                StringHelper.IsEqualIgnoreCase(item.EstimateId, request.EstimateId), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.EstimateItem));

        var estimateItemWithSameLabel = await _dbContext.EstimateItems
            .AnyAsync(item =>
                !StringHelper.IsEqualIgnoreCase(item.Id, existingEstimateItem.Id) &&
                StringHelper.IsEqualIgnoreCase(item.EstimateId, request.EstimateId) &&
                StringHelper.IsEqualIgnoreCase(item.Label, request.Label), cancellationToken);
        if (estimateItemWithSameLabel)
            throw new QuillityException(
                StatusCodes.BAD_REQUEST,
                ErrorMessages.GetEntryWithSamePropExistsError(ErrorMessageModel.EstimateItem, "label", ErrorMessageScope.Workspace));

        var estimateItemWithSameMagnitude = await _dbContext.EstimateItems
            .AnyAsync(item =>
                !StringHelper.IsEqualIgnoreCase(item.Id, existingEstimateItem.Id) &&
                StringHelper.IsEqualIgnoreCase(item.EstimateId, request.EstimateId) &&
                item.MagnitudeIndex == request.MagnitudeIndex, cancellationToken);
        if (estimateItemWithSameMagnitude)
            throw new QuillityException(
                StatusCodes.BAD_REQUEST,
                ErrorMessages.GetEntryWithSamePropExistsError(ErrorMessageModel.EstimateItem, "magnitude", ErrorMessageScope.Workspace));

        existingEstimateItem.Label = request.Label;
        existingEstimateItem.MagnitudeIndex = request.MagnitudeIndex;

        _dbContext.EstimateItems.Update(existingEstimateItem);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToEstimateItemDTO(existingEstimateItem);
    }
}
