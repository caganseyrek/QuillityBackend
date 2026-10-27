using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Common.Exceptions;
using Quillity.Infrastructure;
using Quillity.Auth;
using Quillity.Common.Values;
using Quillity.Common.Helpers;

namespace Quillity.Application.Workspaces.Labels.Queries;

public class GetLabelsQuery : IRequest<List<LabelDTO>>
{
    public Guid WorkspaceId { get; set; }
}

public class GetLabelsQueryHandler : IRequestHandler<GetLabelsQuery, List<LabelDTO>>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public GetLabelsQueryHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<List<LabelDTO>> Handle(GetLabelsQuery request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.None,
            cancellationToken);

        var labels = _dbContext.Labels
            .Where(label => StringHelper.IsEqualIgnoreCase(label.WorkspaceId, workspace.Id))
            .Select(ObjectMapper.ToLabelDTO)
            .ToList();

        return labels;
    }
}
