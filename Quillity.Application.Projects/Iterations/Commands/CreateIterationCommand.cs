using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Models;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.Iterations.Commands;

public class CreateIterationCommandBody
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public long StartDateMs { get; set; }
    public long TargetDateMs { get; set; }
    public long EndDateMs { get; set; }
    public string? EndGoal { get; set; }
    public int VelocityPoints { get; set; }
    public string? Restrospective { get; set; }
    public Guid StatusId { get; set; }
}

public class CreateIterationCommand : CreateIterationCommandBody, IRequest<IterationDTO>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
}

public class CreateIterationCommandHandler : IRequestHandler<CreateIterationCommand, IterationDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public CreateIterationCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<IterationDTO> Handle(CreateIterationCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.None,
            cancellationToken);

        var project = await _permissionService.AuthorizeProjectAccessAsync(
            workspaceId: workspace.Id,
            projectId: request.ProjectId,
            requiredPermission: ProjectPermissions.CreateIteration,
            cancellationToken);

        var status = await _dbContext.Statuses
            .FirstOrDefaultAsync(status =>
                StringHelper.IsEqualIgnoreCase(status.WorkspaceId, workspace.Id) &&
                StringHelper.IsEqualIgnoreCase(status.Id, request.StatusId), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.Status));

        var newIteration = new IterationModel
        {
            Project = project,
            ProjectId = project.Id,
            Name = request.Name,
            Description = request.Description,
            StartDateMs = request.StartDateMs,
            TargetDateMs = request.TargetDateMs,
            EndDateMs = request.EndDateMs,
            EndGoal = request.EndGoal,
            PlannedItemCount = 0,
            CompletedItemCount = 0,
            VelocityPoints = request.VelocityPoints,
            Status = status,
            StatusId = status.Id,
        };

        await _dbContext.Iterations.AddAsync(newIteration, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToIterationDTO(newIteration);
    }
}
