using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Estimates.Commands;

public class DeleteEstimateCommand : IRequest<Unit>
{
    public Guid WorkspaceId { get; set; }
    public Guid EstimateId { get; set; }
}

public class DeleteEstimateCommandHandler : IRequestHandler<DeleteEstimateCommand, Unit>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public DeleteEstimateCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<Unit> Handle(DeleteEstimateCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.DeleteEstimate,
            cancellationToken);

        var existingEstimate = await _dbContext.Estimates
            .FirstOrDefaultAsync(estimate =>
                StringHelper.IsEqualIgnoreCase(estimate.WorkspaceId, workspace.Id) &&
                StringHelper.IsEqualIgnoreCase(estimate.Id, estimate.Id), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.Estimate));

        _dbContext.Estimates.Remove(existingEstimate);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
