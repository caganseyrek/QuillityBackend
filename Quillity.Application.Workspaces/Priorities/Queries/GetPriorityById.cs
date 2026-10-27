using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Priorities.Queries;

public class GetPriorityByIdQuery : IRequest<PriorityDTO>
{
    public Guid PriorityId { get; set; }
    public Guid WorkspaceId { get; set; }
}

public class GetPriorityByIdQueryHandler : IRequestHandler<GetPriorityByIdQuery, PriorityDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public GetPriorityByIdQueryHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<PriorityDTO> Handle(GetPriorityByIdQuery request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.None,
            cancellationToken);

        var priority = await _dbContext.Priorities
            .FirstOrDefaultAsync(priority =>
                StringHelper.IsEqualIgnoreCase(priority.Id, request.PriorityId) &&
                StringHelper.IsEqualIgnoreCase(priority.WorkspaceId, workspace.Id), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.Priority));

        return ObjectMapper.ToPriorityDTO(priority);
    }
}
