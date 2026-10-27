using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Statuses.Queries;

public class GetStatusByIdQuery : IRequest<StatusDTO>
{
    public Guid WorkspaceId { get; set; }
    public Guid StatusId { get; set; }
}

public class GetStatusByIdQueryHandler : IRequestHandler<GetStatusByIdQuery, StatusDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public GetStatusByIdQueryHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<StatusDTO> Handle(GetStatusByIdQuery request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.None,
            cancellationToken);

        var existingStatus = await _dbContext.Statuses
            .FirstOrDefaultAsync(status =>
                StringHelper.IsEqualIgnoreCase(status.Id, request.StatusId) &&
                StringHelper.IsEqualIgnoreCase(status.WorkspaceId, workspace.Id), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.Status));

        return ObjectMapper.ToStatusDTO(existingStatus);
    }
}