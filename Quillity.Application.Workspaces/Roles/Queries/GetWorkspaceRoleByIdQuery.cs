using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Roles.Queries;

public class GetWorkspaceRoleByIdQuery : IRequest<WorkspaceRoleDTO>
{
    public Guid WorkspaceId { get; set; }
    public Guid RoleId { get; set; }
}

public class GetWorkspaceRoleByIdQueryHandler : IRequestHandler<GetWorkspaceRoleByIdQuery, WorkspaceRoleDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public GetWorkspaceRoleByIdQueryHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<WorkspaceRoleDTO> Handle(GetWorkspaceRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.None,
            cancellationToken);

        var workspaceRole = await _dbContext.WorkspaceRoles
            .FirstOrDefaultAsync(role =>
                StringHelper.IsEqualIgnoreCase(role.WorkspaceId, workspace.Id) &&
                StringHelper.IsEqualIgnoreCase(role.Id, request.RoleId), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.WorkspaceRole));

        return ObjectMapper.ToWorkspaceRoleDTO(workspaceRole);
    }
}
