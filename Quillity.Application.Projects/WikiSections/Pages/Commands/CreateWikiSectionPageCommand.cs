using MediatR;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Models;
using Quillity.Common.Response;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.WikiSections.Pages.Commands;

public class CreateWikiSectionPageCommandBody
{
    public required string Title { get; set; }
    public required string Content { get; set; }
}

public class CreateWikiSectionPageCommand : CreateWikiSectionPageCommandBody, IRequest<Unit>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid WikiSectionId { get; set; }
}

public class CreateWikiSectionPageCommandHandler : IRequestHandler<CreateWikiSectionPageCommand, Unit>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _workspaceService;
    private readonly ProjectService _projectService;

    public CreateWikiSectionPageCommandHandler(
        QuillityDbContext dbContext,
        PermissionService workspaceService,
        ProjectService projectService)
    {
        _dbContext = dbContext;
        _workspaceService = workspaceService;
        _projectService = projectService;
    }

    public async Task<Unit> Handle(CreateWikiSectionPageCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _workspaceService.GetWorkspaceIfUserCanAccessAsync(request.WorkspaceId, cancellationToken);
        var project = await _projectService.GetProjectIfUserCanAccessAsync(
            workspaceId: workspace.Id,
            projectId: request.ProjectId,
            includeWikiSections: true,
            cancellationToken: cancellationToken);

        if (!_projectService.CanUserManageWikiPages(project))
            throw new QuillityException(StatusCodes.UNAUTHORIZED, "You do not have permission to manage wiki pages in this project.");

        var wikiSection = project.WikiSections.FirstOrDefault(section => section.Id.Equals(request.WikiSectionId))
            ?? throw new QuillityException(StatusCodes.NOT_FOUND, "Wiki section not found.");

        var pageWithSameName = wikiSection.Pages.FirstOrDefault(page => page.Title.Equals(request.Title));
        if (pageWithSameName is not null)
            throw new QuillityException(StatusCodes.BAD_REQUEST, "A page with the same title already exists in this wiki.");

        var newPage = new WikiSectionPageModel
        {
            WikiSectionId = wikiSection.Id,
            WikiSection = wikiSection,
            Title = request.Title,
            Content = request.Content,
        };

        return Unit.Value;
    }
}
