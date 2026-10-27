using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Models;
using Quillity.Common.Presets;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.Core.Commands;

public class CreateProjectCommandBody
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string UrlKey { get; set; }
    public required string TimeZone { get; set; }
    public required string IconName { get; set; }
}

public class CreateProjectCommand : CreateProjectCommandBody, IRequest<ProjectDTO>
{
    public Guid WorkspaceId { get; set; }
}

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public CreateProjectCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<ProjectDTO> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.CreateProject,
            cancellationToken);

        var projectWithSameUrlKey = await _dbContext.Projects
            .AnyAsync(project =>
                StringHelper.IsEqualIgnoreCase(project.WorkspaceId, workspace.Id) &&
                StringHelper.IsEqualIgnoreCase(project.UrlKey, request.UrlKey), cancellationToken);
        if (projectWithSameUrlKey)
            throw new QuillityException(
                StatusCodes.BAD_REQUEST,
                ErrorMessages.GetEntryWithSamePropExistsError(ErrorMessageModel.Project, "url key", ErrorMessageScope.Workspace));

        var projectWithSameName = await _dbContext.Projects
            .AnyAsync(project =>
                StringHelper.IsEqualIgnoreCase(project.WorkspaceId, workspace.Id) &&
                StringHelper.IsEqualIgnoreCase(project.Name, request.Name), cancellationToken);
        if (projectWithSameUrlKey)
            throw new QuillityException(
                StatusCodes.BAD_REQUEST,
                ErrorMessages.GetEntryWithSamePropExistsError(ErrorMessageModel.Project, "name", ErrorMessageScope.Workspace));

        var newProject = new ProjectModel
        {
            Workspace = workspace,
            WorkspaceId = request.WorkspaceId,
            Name = request.Name,
            Description = request.Description,
            UrlKey = request.UrlKey,
            TimeZone = request.TimeZone,
            IconName = request.IconName,
        };

        var projectPresets = new ProjectPresets(newProject);
        var leadRole = projectPresets.GetLeadRole();
        var contributorRole = projectPresets.GetContributorRole();

        newProject.LeadRole = leadRole;
        newProject.LeadRoleId = leadRole.Id;

        newProject.DefaultRole = contributorRole;
        newProject.DefaultRoleId = contributorRole.Id;

        var projectRoles = new List<ProjectRoleModel>
        {
            leadRole,
            contributorRole,
        };

        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await _dbContext.Projects.AddAsync(newProject, cancellationToken);
            await _dbContext.ProjectRoles.AddRangeAsync(projectRoles, cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new QuillityException(
                StatusCodes.SERVER_ERROR,
                ErrorMessages.GetServerError());
        }

        return ObjectMapper.ToProjectDTO(newProject);
    }
}
