using MediatR;
using Quillity.Auth;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Core.Commands;

public class DeleteWorkspaceCommand : IRequest<Unit>
{
    public required Guid WorkspaceId { get; set; }
}

public class DeleteWorkspaceCommandHandler : IRequestHandler<DeleteWorkspaceCommand, Unit>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public DeleteWorkspaceCommandHandler(
        QuillityDbContext dbContext,
        PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<Unit> Handle(DeleteWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.DeleteWorkspace,
            cancellationToken);

        _dbContext.Workspaces.Remove(workspace);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}
