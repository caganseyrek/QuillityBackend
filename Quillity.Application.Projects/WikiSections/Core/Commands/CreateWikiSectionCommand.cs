using MediatR;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Models;
using Quillity.Common.Response;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.WikiSections.Core.Commands;

public class CreateWikiSectionCommandBody
{
    public required string Name { get; set; }
    public required string Description { get; set; }
}

public class CreateWikiSectionCommand : CreateWikiSectionCommandBody, IRequest<Unit>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
}

public class CreateWikiSectionCommandHandler : IRequestHandler<CreateWikiSectionCommand, Unit>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _workspaceService;
    private readonly ProjectService _projectService;

    public CreateWikiSectionCommandHandler(
        QuillityDbContext dbContext,
        PermissionService workspaceService,
        ProjectService projectService)
    {
        _dbContext = dbContext;
        _workspaceService = workspaceService;
        _projectService = projectService;
    }

    public async Task<Unit> Handle(CreateWikiSectionCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _workspaceService.GetWorkspaceIfUserCanAccessAsync(request.WorkspaceId, cancellationToken);
        var project = await _projectService.GetProjectIfUserCanAccessAsync(
            workspaceId: workspace.Id,
            projectId: request.ProjectId,
            includeWikiSections: true,
            cancellationToken: cancellationToken);

        if (!_projectService.CanUserManageWikiPages(project))
            throw new QuillityException(StatusCodes.UNAUTHORIZED, "You do not have permission to manage wiki pages in this project.");

        var wikiSectionWithSameName = project.WikiSections.FirstOrDefault(wikiSection => wikiSection.Name.Equals(request.Name));
        if (wikiSectionWithSameName is not null)
            throw new QuillityException(StatusCodes.BAD_REQUEST, "A section with the same name already exists in this project wiki.");

        var newSection = new WikiSectionModel
        {
            ProjectId = project.Id,
            Project = project,
            Name = request.Name,
            Description = request.Description,
        };

        await _dbContext.WikiSections.AddAsync(newSection, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
