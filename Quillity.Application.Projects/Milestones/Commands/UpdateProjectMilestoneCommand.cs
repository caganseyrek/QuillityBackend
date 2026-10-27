using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.Milestones.Commands;

public class UpdateProjectMilestoneCommandBody
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public long DueDateMs { get; set; }
    public int ProgressPercentage { get; set; }
}

public class UpdateProjectMilestoneCommand : UpdateProjectMilestoneCommandBody, IRequest<MilestoneDTO>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid MilestoneId { get; set; }
}

public class UpdateProjectMilestoneCommandHandler : IRequestHandler<UpdateProjectMilestoneCommand, MilestoneDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public UpdateProjectMilestoneCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<MilestoneDTO> Handle(UpdateProjectMilestoneCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.None,
            cancellationToken);

        var project = await _permissionService.AuthorizeProjectAccessAsync(
            workspaceId: workspace.Id,
            projectId: request.ProjectId,
            requiredPermission: ProjectPermissions.UpdateMilestone,
            cancellationToken);

        var existingMilestone = await _dbContext.Milestones
            .FirstOrDefaultAsync(milestone =>
                StringHelper.IsEqualIgnoreCase(milestone.Id, request.MilestoneId) &&
                StringHelper.IsEqualIgnoreCase(milestone.ProjectId, project.Id), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.Milestone));

        _dbContext.Milestones.Update(existingMilestone);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToMilestoneDTO(existingMilestone);
    }
}
