using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.Milestones.Commands;

public class DeleteProjectMilestoneCommandBody
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public long DueDateMs { get; set; }
    public int ProgressPercentage { get; set; }
}

public class DeleteProjectMilestoneCommand : DeleteProjectMilestoneCommandBody, IRequest<Unit>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid MilestoneId { get; set; }
}

public class DeleteProjectMilestoneCommandHandler : IRequestHandler<DeleteProjectMilestoneCommand, Unit>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public DeleteProjectMilestoneCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<Unit> Handle(DeleteProjectMilestoneCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.None,
            cancellationToken);

        var project = await _permissionService.AuthorizeProjectAccessAsync(
            workspaceId: workspace.Id,
            projectId: request.ProjectId,
            requiredPermission: ProjectPermissions.DeleteMilestone,
            cancellationToken);

        var existingMilestone = await _dbContext.Milestones
            .FirstOrDefaultAsync(milestone =>
                StringHelper.IsEqualIgnoreCase(milestone.Id, request.MilestoneId) &&
                StringHelper.IsEqualIgnoreCase(milestone.ProjectId, project.Id), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.Milestone));

        _dbContext.Milestones.Remove(existingMilestone);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
