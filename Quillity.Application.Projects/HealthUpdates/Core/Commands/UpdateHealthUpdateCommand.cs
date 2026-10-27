using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.HealthUpdates.Core.Commands;

public class UpdateHealthUpdateCommandBody
{
    public required string Summary { get; set; }
    public HealthUpdateStatus HealthStatus { get; set; }
}

public class UpdateHealthUpdateCommand : UpdateHealthUpdateCommandBody, IRequest<HealthUpdateDTO>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid HealthUpdateId { get; set; }
}

public class UpdateHealthUpdateCommandHandler : IRequestHandler<UpdateHealthUpdateCommand, HealthUpdateDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public UpdateHealthUpdateCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<HealthUpdateDTO> Handle(UpdateHealthUpdateCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.None,
            cancellationToken);

        var project = await _permissionService.AuthorizeProjectAccessAsync(
            workspaceId: workspace.Id,
            projectId: request.ProjectId,
            requiredPermission: ProjectPermissions.UpdateHealthUpdate,
            cancellationToken);

        var existingHealthUpdate = await _dbContext.HealthUpdates
            .FirstOrDefaultAsync(update =>
                StringHelper.IsEqualIgnoreCase(update.ProjectId, project.Id) &&
                StringHelper.IsEqualIgnoreCase(update.Id, request.HealthUpdateId), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.HealthUpdate));

        existingHealthUpdate.Status = request.HealthStatus;
        existingHealthUpdate.Summary = request.Summary;

        _dbContext.HealthUpdates.Update(existingHealthUpdate);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToHealthUpdateDTO(existingHealthUpdate);
    }
}
