using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Response;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.WikiSections.Pages.Commands;

public class UpdateWikiSectionPageCommandBody
{
    public required string Title { get; set; }
    public required string Content { get; set; }
}

public class UpdateWikiSectionPageCommand : UpdateWikiSectionPageCommandBody, IRequest<Unit>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid WikiSectionId { get; set; }
    public Guid PageId { get; set; }
}

public class UpdateWikiSectionPageCommandHandler : IRequestHandler<UpdateWikiSectionPageCommand, Unit>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _workspaceService;
    private readonly ProjectService _projectService;

    public UpdateWikiSectionPageCommandHandler(
        QuillityDbContext dbContext,
        PermissionService workspaceService,
        ProjectService projectService)
    {
        _dbContext = dbContext;
        _workspaceService = workspaceService;
        _projectService = projectService;
    }

    public async Task<Unit> Handle(UpdateWikiSectionPageCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _workspaceService.GetWorkspaceIfUserCanAccessAsync(request.WorkspaceId, cancellationToken);
        var project = await _projectService.GetProjectIfUserCanAccessAsync(
            workspaceId: workspace.Id,
            projectId: request.ProjectId,
            includeWikiSections: true,
            cancellationToken: cancellationToken);

        if (!_projectService.CanUserManageWikiPages(project))
            throw new QuillityException(StatusCodes.UNAUTHORIZED, "You do not have permission to manage wiki pages in this project.");

        var section = project.WikiSections.FirstOrDefault(section => section.Id.Equals(request.WikiSectionId))
            ?? throw new QuillityException(StatusCodes.NOT_FOUND, "Wiki section not found.");
        
        var page = section.Pages.FirstOrDefault(page => page.Id.Equals(request.PageId))
            ?? throw new QuillityException(StatusCodes.NOT_FOUND, "Wiki section page not found.");
        
        var pageWithSameTitle = section.Pages.FirstOrDefault(page => page.Title.Equals(request.Title) && !page.Id.Equals(page.Id));
        if (pageWithSameTitle is not null)
            throw new QuillityException(StatusCodes.BAD_REQUEST, "A page with the same title already exists in this wiki section.");

        page.Title = request.Title;
        page.Content = request.Content;
        
        _dbContext.WikiSectionPages.Update(page);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}