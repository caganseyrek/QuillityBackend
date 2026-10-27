using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Labels.Commands;

public class DeleteLabelCommand : IRequest<Unit>
{
    public Guid WorkspaceId { get; set; }
    public Guid LabelId { get; set; }
}

public class DeleteLabelCommandHandler : IRequestHandler<DeleteLabelCommand, Unit>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public DeleteLabelCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<Unit> Handle(DeleteLabelCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.DeleteLabel,
            cancellationToken);

        var existingLabel = await _dbContext.Labels
            .FirstOrDefaultAsync(label =>
                StringHelper.IsEqualIgnoreCase(label.WorkspaceId, workspace.Id) &&
                StringHelper.IsEqualIgnoreCase(label.Id, request.LabelId), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.UNAUTHORIZED,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.Label));

        _dbContext.Labels.Remove(existingLabel);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
