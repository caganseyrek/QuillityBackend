using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Labels.Queries;

public class GetLabelByIdQuery : IRequest<LabelDTO>
{
    public Guid WorkspaceId { get; set; }
    public Guid LabelId { get; set; }
}

public class GetLabelByIdQueryHandler : IRequestHandler<GetLabelByIdQuery, LabelDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;
    
    public GetLabelByIdQueryHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<LabelDTO> Handle(GetLabelByIdQuery request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.None,
            cancellationToken);

        var existingLabel = await _dbContext.Labels
            .FirstOrDefaultAsync(label =>
                StringHelper.IsEqualIgnoreCase(label.Id, request.LabelId) &&
                StringHelper.IsEqualIgnoreCase(label.WorkspaceId, workspace.Id), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.Label));

        return ObjectMapper.ToLabelDTO(existingLabel);
    }
}
