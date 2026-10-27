using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Context;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Models;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Members.Commands;

public class SendWorkspaceInvitationCommandBody
{
    public required string InvitedUserEmail { get; set; }
    public required string InvitationNote { get; set; }
}

public class SendWorkspaceInvitationCommand : SendWorkspaceInvitationCommandBody, IRequest<InvitationDTO>
{
    public Guid WorkspaceId { get; set; }
}

public class SendWorkspaceInvitationCommandHandler : IRequestHandler<SendWorkspaceInvitationCommand, InvitationDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly UserContext _userContext;
    private readonly PermissionService _permissionService;

    public SendWorkspaceInvitationCommandHandler(
        QuillityDbContext dbContext,
        UserContext userContext,
        PermissionService permissionService)
    {
        _dbContext = dbContext;
        _userContext = userContext;
        _permissionService = permissionService;
    }

    public async Task<InvitationDTO> Handle(SendWorkspaceInvitationCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.SendWorkspaceInvitation,
            cancellationToken);

        var invitedUser = await _dbContext.Users
            .FirstOrDefaultAsync(user => StringHelper.IsEqualIgnoreCase(user.Email, request.InvitedUserEmail), cancellationToken)
            ?? throw new QuillityException(StatusCodes.NOT_FOUND, "User not found with the provided email.");

        var isAlreadyMember = workspace.Members
            .Any(member =>
                StringHelper.IsEqualIgnoreCase(member.UserId, invitedUser.Id) &&
                StringHelper.IsEqualIgnoreCase(member.User.Email, invitedUser.Email));
        if (isAlreadyMember)
            throw new QuillityException(StatusCodes.BAD_REQUEST, "User is already a member of this workspace.");

        var newInvitation = new InvitationModel
        {
            InvitedByUserId = _userContext.User.Id,
            InvitedByUser = _userContext.User,
            InvitedUserId = invitedUser.Id,
            InvitedUser = invitedUser,
            InvitedWorkspaceId = workspace.Id,
            InvitedWorkspace = workspace,
            InvitationNote = request.InvitationNote,
        };

        await _dbContext.Invitations.AddAsync(newInvitation, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToInvitationDTO(newInvitation);
    }
}
