using MediatR;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Members.Commands;

public class LeaveWorkspaceCommand : IRequest<Unit>
{
    public Guid WorkspaceId { get; set; }
}

public class LeaveWorkspaceCommandHandler : IRequestHandler<LeaveWorkspaceCommand, Unit>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public LeaveWorkspaceCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<Unit> Handle(LeaveWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.None,
            cancellationToken);

        var userMembership = workspace.Members.FirstOrDefault()
            ?? throw new Exception("User was not found in workspace after 'AuthorizeWorkspaceAccessAsync' check. This indicates a logic error.");

        if (StringHelper.IsEqualIgnoreCase(workspace.OwnerRoleId, userMembership.Role.Id))
            throw new QuillityException(
                StatusCodes.FORBIDDEN,
                "You cannot leave a workspace that you own. You must transfer ownership first.");

        _dbContext.WorkspaceMembers.Remove(userMembership);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
