using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.Milestones.Queries;

public class GetProjectMilestoneByIdQuery : IRequest<MilestoneDTO>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid MilestoneId { get; set; }
}

public class GetProjectMilestoneByIdQueryHandler : IRequestHandler<GetProjectMilestoneByIdQuery, MilestoneDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public GetProjectMilestoneByIdQueryHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<MilestoneDTO> Handle(GetProjectMilestoneByIdQuery request, CancellationToken cancellationToken)
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

        var existingMilestone = await _dbContext.Milestones
            .FirstOrDefaultAsync(milestone =>
                StringHelper.IsEqualIgnoreCase(milestone.Id, request.MilestoneId) &&
                StringHelper.IsEqualIgnoreCase(milestone.ProjectId, project.Id), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.Milestone));

        return ObjectMapper.ToMilestoneDTO(existingMilestone);
    }
}
