using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Statuses.Commands;

public class DeleteStatusCommand : IRequest<Unit>
{
    public Guid WorkspaceId { get; set; }
    public Guid StatusId { get; set; }
}

public class DeleteStatusCommandHandler : IRequestHandler<DeleteStatusCommand, Unit>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public DeleteStatusCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<Unit> Handle(DeleteStatusCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.DeleteStatus,
            cancellationToken);

        var existingStatus = await _dbContext.Statuses
            .FirstOrDefaultAsync(status =>
                StringHelper.IsEqualIgnoreCase(status.Id, request.StatusId) &&
                StringHelper.IsEqualIgnoreCase(status.WorkspaceId, workspace.Id), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.Status));

        var hasOtherStatusesInGroup = await _dbContext.Statuses
            .AnyAsync(status =>
                !StringHelper.IsEqualIgnoreCase(status.Id, request.StatusId) &&
                StringHelper.IsEqualIgnoreCase(status.WorkspaceId, workspace.Id) &&
                status.Group == existingStatus.Group, cancellationToken);
        if (!hasOtherStatusesInGroup)
            throw new QuillityException(
                StatusCodes.BAD_REQUEST,
                "Each status group should have at least one status.");

        _dbContext.Statuses.Remove(existingStatus);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
