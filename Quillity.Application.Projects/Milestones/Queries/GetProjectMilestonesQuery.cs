using MediatR;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Helpers;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.Milestones.Queries;

public class GetProjectMilestonesQuery : IRequest<List<MilestoneDTO>>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid MilestoneId { get; set; }
}

public class GetProjectMilestonesQueryHandler : IRequestHandler<GetProjectMilestonesQuery, List<MilestoneDTO>>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public GetProjectMilestonesQueryHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<List<MilestoneDTO>> Handle(GetProjectMilestonesQuery request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.None,
            cancellationToken);

        var project = await _permissionService.AuthorizeProjectAccessAsync(
            workspaceId: workspace.Id,
            projectId: request.ProjectId,
            requiredPermission: ProjectPermissions.None,
            cancellationToken);

        var existingMilestones = _dbContext.Milestones
            .Where(milestone =>
                StringHelper.IsEqualIgnoreCase(milestone.Id, request.MilestoneId) &&
                StringHelper.IsEqualIgnoreCase(milestone.ProjectId, project.Id))
            .Select(ObjectMapper.ToMilestoneDTO)
            .ToList();

        return existingMilestones;
    }
}
