using MediatR;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Helpers;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Members.Queries;

public class GetWorkspaceInvitationsQuery : IRequest<List<InvitationDTO>>
{
    public Guid WorkspaceId { get; set; }
}

public class GetWorkspaceInvitationsQueryHandler : IRequestHandler<GetWorkspaceInvitationsQuery, List<InvitationDTO>>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public GetWorkspaceInvitationsQueryHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<List<InvitationDTO>> Handle(GetWorkspaceInvitationsQuery request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.ViewWorkspaceInvitations,
            cancellationToken);

        var invitationDTOs = _dbContext.Invitations
            .Where(invitation => StringHelper.IsEqualIgnoreCase(invitation.InvitedWorkspaceId, request.WorkspaceId))
            .Select(ObjectMapper.ToInvitationDTO)
            .ToList();

        return invitationDTOs;
    }
}
