using MediatR;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Response;

namespace Quillity.Application.Projects.WikiSections.Pages.Queries;

public class GetWikiSectionPagesQuery : IRequest<List<WikiSectionPageDTO>>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid WikiSectionId { get; set; }
}

public class GetProjectWikiSectionPagesQueryHandler : IRequestHandler<GetWikiSectionPagesQuery, List<WikiSectionPageDTO>>
{
    private readonly PermissionService _workspaceService;
    private readonly ProjectService _projectService;

    public GetProjectWikiSectionPagesQueryHandler(PermissionService workspaceService, ProjectService projectService)
    {
        _workspaceService = workspaceService;
        _projectService = projectService;
    }

    public async Task<List<WikiSectionPageDTO>> Handle(GetWikiSectionPagesQuery request, CancellationToken cancellationToken)
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

        var pageDTOs = section.Pages.Select(page => new WikiSectionPageDTO
        {
            Id = page.Id,
            Title = page.Title,
            Content = page.Content,
            IsDeprecated = page.IsDeprecated,
        }).ToList();

        return pageDTOs;
    }
}
