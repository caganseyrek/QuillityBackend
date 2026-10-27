using MediatR;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Response;

namespace Quillity.Application.Projects.WikiSections.Pages.Queries;

public class GetWikiSectionPageByIdQuery : IRequest<WikiSectionPageDTO>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid WikiSectionId { get; set; }
    public Guid PageId { get; set; }
}

public class GetProjectWikiSectionPageByIdQueryHandler : IRequestHandler<GetWikiSectionPageByIdQuery, WikiSectionPageDTO>
{
    private readonly PermissionService _workspaceService;
    private readonly ProjectService _projectService;

    public GetProjectWikiSectionPageByIdQueryHandler(PermissionService workspaceService, ProjectService projectService)
    {
        _workspaceService = workspaceService;
        _projectService = projectService;
    }

    public async Task<WikiSectionPageDTO> Handle(GetWikiSectionPageByIdQuery request, CancellationToken cancellationToken)
    {
        var workspace = await _workspaceService.GetWorkspaceIfUserCanAccessAsync(request.WorkspaceId, cancellationToken);
        var project = await _projectService.GetProjectIfUserCanAccessAsync(
            workspaceId: workspace.Id,
            projectId: request.ProjectId,
            includeWikiSections: true,
            cancellationToken: cancellationToken);

        var section = project.WikiSections
            .FirstOrDefault(section => section.Id.Equals(request.WikiSectionId) && section.ProjectId.Equals(project.Id))
            ?? throw new QuillityException(StatusCodes.NOT_FOUND, "Wiki section not found.");
        
        var page = section.Pages.FirstOrDefault(page => page.Id.Equals(request.PageId))
            ?? throw new QuillityException(StatusCodes.NOT_FOUND, "Wiki section page not found.");
        
        return new WikiSectionPageDTO
        {
            Id = page.Id,
            Title = page.Title,
            Content = page.Content,
            IsDeprecated = page.IsDeprecated,
        };
    }
}
