using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.Core.Commands;

public class UpdateProjectCommandBody
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string UrlKey { get; set; }
    public required string TimeZone { get; set; }
    public required string IconName { get; set; }
}

public class UpdateProjectCommand : UpdateProjectCommandBody, IRequest<ProjectDTO>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
}

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, ProjectDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public UpdateProjectCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<ProjectDTO> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.None,
            cancellationToken);

        var project = await _permissionService.AuthorizeProjectAccessAsync(
            workspaceId: workspace.Id,
            projectId: request.ProjectId,
            requiredPermission: ProjectPermissions.UpdateProject,
            cancellationToken);

        var projectWithSameUrlKey = await _dbContext.Projects
            .AnyAsync(project =>
                !StringHelper.IsEqualIgnoreCase(project.Id, request.ProjectId) &&
                StringHelper.IsEqualIgnoreCase(project.WorkspaceId, workspace.Id) &&
                StringHelper.IsEqualIgnoreCase(project.UrlKey, request.UrlKey), cancellationToken);
        if (projectWithSameUrlKey)
            throw new QuillityException(
                StatusCodes.BAD_REQUEST,
                ErrorMessages.GetEntryWithSamePropExistsError(ErrorMessageModel.Project, "url key", ErrorMessageScope.Workspace));

        var projectWithSameName = await _dbContext.Projects
            .AnyAsync(project =>
                !StringHelper.IsEqualIgnoreCase(project.Id, request.ProjectId) &&
                StringHelper.IsEqualIgnoreCase(project.WorkspaceId, workspace.Id) &&
                StringHelper.IsEqualIgnoreCase(project.Name, request.Name), cancellationToken);
        if (projectWithSameUrlKey)
            throw new QuillityException(
                StatusCodes.BAD_REQUEST,
                ErrorMessages.GetEntryWithSamePropExistsError(ErrorMessageModel.Project, "name", ErrorMessageScope.Workspace));

        project.Name = request.Name;
        project.Description = request.Description;
        project.UrlKey = request.UrlKey;
        project.TimeZone = request.TimeZone;
        project.IconName = request.IconName;

        _dbContext.Projects.Update(project);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToProjectDTO(project);
    }
}
