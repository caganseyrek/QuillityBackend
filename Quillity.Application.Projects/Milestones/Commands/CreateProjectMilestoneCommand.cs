using MediatR;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Models;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.Milestones.Commands;

public class CreateProjectMilestoneCommandBody
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public long DueDateMs { get; set; }
    public int ProgressPercentage { get; set; }
}

public class CreateProjectMilestoneCommand : CreateProjectMilestoneCommandBody, IRequest<MilestoneDTO>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
}

public class CreateProjectMilestoneCommandHandler : IRequestHandler<CreateProjectMilestoneCommand, MilestoneDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public CreateProjectMilestoneCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<MilestoneDTO> Handle(CreateProjectMilestoneCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.None,
            cancellationToken);

        var project = await _permissionService.AuthorizeProjectAccessAsync(
            workspaceId: workspace.Id,
            projectId: request.ProjectId,
            requiredPermission: ProjectPermissions.CreateMilestone,
            cancellationToken);

        var newMilestone = new MilestoneModel
        {
            ProjectId = project.Id,
            Project = project,
            Name = request.Name,
            Description = request.Description,
            DueDateMs = request.DueDateMs,
            ProgressPercentage = request.ProgressPercentage,
        };

        await _dbContext.Milestones.AddAsync(newMilestone, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToMilestoneDTO(newMilestone);
    }
}
