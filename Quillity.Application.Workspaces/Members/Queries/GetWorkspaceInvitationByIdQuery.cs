using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Members.Queries;

public class GetWorkspaceInvitationByIdQuery : IRequest<InvitationDTO>
{
    public Guid WorkspaceId { get; set; }
    public Guid InvitationId { get; set; }
}

public class GetWorkspaceInvitationByIdQueryHandler : IRequestHandler<GetWorkspaceInvitationByIdQuery, InvitationDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public GetWorkspaceInvitationByIdQueryHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<InvitationDTO> Handle(GetWorkspaceInvitationByIdQuery request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.ViewWorkspaceInvitations,
            cancellationToken);

        var invitation = await _dbContext.Invitations
            .FirstOrDefaultAsync(invitation =>
                StringHelper.IsEqualIgnoreCase(invitation.Id, request.InvitationId) &&
                StringHelper.IsEqualIgnoreCase(invitation.InvitedWorkspaceId, workspace.Id), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.Invitation));

        return ObjectMapper.ToInvitationDTO(invitation);
    }
}
