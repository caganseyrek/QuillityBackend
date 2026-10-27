using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.Iterations.Commands;

public class UpdateIterationCommandBody
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

public class UpdateIterationCommand : UpdateIterationCommandBody, IRequest<IterationDTO>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid IterationId { get; set; }
}

public class UpdateIterationCommandHandler : IRequestHandler<UpdateIterationCommand, IterationDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public UpdateIterationCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<IterationDTO> Handle(UpdateIterationCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.None,
            cancellationToken);

        var project = await _permissionService.AuthorizeProjectAccessAsync(
            workspaceId: workspace.Id,
            projectId: request.ProjectId,
            requiredPermission: ProjectPermissions.UpdateIteration,
            cancellationToken);

        var existingIteration = await _dbContext.Iterations
            .FirstOrDefaultAsync(iteration =>
                StringHelper.IsEqualIgnoreCase(iteration.Id, request.IterationId) &&
                StringHelper.IsEqualIgnoreCase(iteration.ProjectId, project.Id), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.Iteration));

        if (!StringHelper.IsEqualIgnoreCase(existingIteration.StatusId, request.StatusId))
        {
            var status = await _dbContext.Statuses
                .FirstOrDefaultAsync(status =>
                    StringHelper.IsEqualIgnoreCase(status.WorkspaceId, workspace.Id) &&
                    StringHelper.IsEqualIgnoreCase(status.Id, request.StatusId), cancellationToken)
                ?? throw new QuillityException(
                    StatusCodes.NOT_FOUND,
                    ErrorMessages.GetNotFoundError(ErrorMessageModel.Status));

            existingIteration.Status = status;
            existingIteration.StatusId = status.Id;
        }

        existingIteration.Name = request.Name;
        existingIteration.Description = request.Description;
        existingIteration.StartDateMs = request.StartDateMs;
        existingIteration.TargetDateMs = request.TargetDateMs;
        existingIteration.EndDateMs = request.EndDateMs;
        existingIteration.EndGoal = request.EndGoal;
        existingIteration.VelocityPoints = request.VelocityPoints;
        existingIteration.Restrospective = request.Restrospective;

        _dbContext.Iterations.Update(existingIteration);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToIterationDTO(existingIteration);
    }
}
