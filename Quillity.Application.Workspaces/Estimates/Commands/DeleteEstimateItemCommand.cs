using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Estimates.Commands;

public class DeleteEstimateItemCommand : IRequest<Unit>
{
    public Guid WorkspaceId { get; set; }
    public Guid EstimateId { get; set; }
    public Guid EstimateItemId { get; set; }
}

public class DeleteEstimateItemCommandHandler : IRequestHandler<DeleteEstimateItemCommand, Unit>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public DeleteEstimateItemCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<Unit> Handle(DeleteEstimateItemCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.DeleteEstimate,
            cancellationToken);

        var existingEstimateItem = await _dbContext.EstimateItems
            .FirstOrDefaultAsync(item =>
                StringHelper.IsEqualIgnoreCase(item.Id, request.EstimateItemId) &&
                StringHelper.IsEqualIgnoreCase(item.EstimateId, request.EstimateId), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.EstimateItem));

        _dbContext.EstimateItems.Remove(existingEstimateItem);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}