using MediatR;
using Quillity.Application.DTOs;
using Quillity.Auth;

namespace Quillity.Application.Projects.WikiSections.Core.Queries;

public class GetWikiSectionsQuery : IRequest<List<WikiSectionDTO>>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
}

public class GetProjectWikiSectionsQueryHandler : IRequestHandler<GetWikiSectionsQuery, List<WikiSectionDTO>>
{
    private readonly PermissionService _workspaceService;
    private readonly ProjectService _projectService;

    public GetProjectWikiSectionsQueryHandler(PermissionService workspaceService, ProjectService projectService)
    {
        _workspaceService = workspaceService;
        _projectService = projectService;
    }

    public async Task<List<WikiSectionDTO>> Handle(GetWikiSectionsQuery request, CancellationToken cancellationToken)
    {
        var workspace = await _workspaceService.GetWorkspaceIfUserCanAccessAsync(request.WorkspaceId, cancellationToken);
        var project = await _projectService.GetProjectIfUserCanAccessAsync(
            workspaceId: workspace.Id,
            projectId: request.ProjectId,
            includeWikiSections: true,
            cancellationToken: cancellationToken);

        var wikiSectionDTOs = project.WikiSections.Select(section => new WikiSectionDTO
        {
            Id = section.Id,
            Name = section.Name,
            Description = section.Description,
            IsDeprecated = section.IsDeprecated,
            IsLocked = section.IsLocked,
        }).ToList();

        return wikiSectionDTOs;
    }
}
