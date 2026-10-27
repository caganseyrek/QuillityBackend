using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Priorities.Commands;

public class DeletePriorityCommand : IRequest<Unit>
{
    public required Guid WorkspaceId { get; set; }
    public required Guid PriorityId { get; set; }
}

public class DeletePriorityCommandHandler : IRequestHandler<DeletePriorityCommand, Unit>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public DeletePriorityCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<Unit> Handle(DeletePriorityCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.DeletePriority,
            cancellationToken);

        var existingPriority = await _dbContext.Priorities
            .FirstOrDefaultAsync(priority =>
                StringHelper.IsEqualIgnoreCase(priority.Id, request.PriorityId) &&
                StringHelper.IsEqualIgnoreCase(priority.WorkspaceId, workspace.Id), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.Priority));

        _dbContext.Priorities.Remove(existingPriority);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
