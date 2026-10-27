using MediatR;
using Quillity.Auth;
using Quillity.Common.Context;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Members.Commands;

public class TransferWorkspaceOwnershipCommandBody
{
    public required Guid NewOwnerId { get; set; }
}

public class TransferWorkspaceOwnershipCommand : TransferWorkspaceOwnershipCommandBody, IRequest<Unit>
{
    public required Guid WorkspaceId { get; set; }
}

public class TransferWorkspaceOwnershipCommandHandler : IRequestHandler<TransferWorkspaceOwnershipCommand, Unit>
{
    private readonly QuillityDbContext _dbContext;
    private readonly UserContext _userContext;
    private readonly PermissionService _permissionService;

    public TransferWorkspaceOwnershipCommandHandler(
        QuillityDbContext dbContext,
        UserContext userContext,
        PermissionService permissionService)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _permissionService = permissionService;
    }

    public async Task<Unit> Handle(TransferWorkspaceOwnershipCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.TransferWorkspaceOwnership,
            cancellationToken);

        var currentUserMembership = workspace.Members.FirstOrDefault()
            ?? throw new Exception("User was not found in workspace after 'AuthorizeWorkspaceAccessAsync' check. This indicates a logic error.");

        if (StringHelper.IsEqualIgnoreCase(workspace.OwnerRoleId, currentUserMembership.RoleId))
            throw new QuillityException(StatusCodes.FORBIDDEN, "You must be the owner of this workspace to transfer ownership.");

        if (StringHelper.IsEqualIgnoreCase(request.NewOwnerId, _userContext.User.Id))
            throw new QuillityException(StatusCodes.BAD_REQUEST, "You are already the owner of this workspace, you cannot transfer the ownership to yourself.");

        var newOwnerMembership = workspace.Members
            .FirstOrDefault(member => StringHelper.IsEqualIgnoreCase(member.UserId, request.NewOwnerId))
            ?? throw new QuillityException(StatusCodes.BAD_REQUEST, "The specified user is not a member of this workspace.");

        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            currentUserMembership.Role = workspace.DefaultRole!;
            currentUserMembership.RoleId = workspace.DefaultRoleId;

            newOwnerMembership.Role = workspace.OwnerRole!;
            newOwnerMembership.RoleId = workspace.OwnerRoleId;

            _dbContext.WorkspaceMembers.Update(currentUserMembership);
            _dbContext.WorkspaceMembers.Update(newOwnerMembership);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new QuillityException(
                StatusCodes.SERVER_ERROR,
                ErrorMessages.GetServerError());
        }
        return Unit.Value;
    }
}
