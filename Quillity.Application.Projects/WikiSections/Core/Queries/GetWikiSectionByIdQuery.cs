using MediatR;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Response;

namespace Quillity.Application.Projects.WikiSections.Core.Queries;

public class GetWikiSectionByIdQuery : IRequest<WikiSectionDTO>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid WikiSectionId { get; set; }
}

public class GetProjectWikiSectionByIdQueryHandler : IRequestHandler<GetWikiSectionByIdQuery, WikiSectionDTO>
{
    private readonly PermissionService _workspaceService;
    private readonly ProjectService _projectService;

    public GetProjectWikiSectionByIdQueryHandler(PermissionService workspaceService, ProjectService projectService)
    {
        _workspaceService = workspaceService;
        _projectService = projectService;
    }

    public async Task<WikiSectionDTO> Handle(GetWikiSectionByIdQuery request, CancellationToken cancellationToken)
    {
        var workspace = await _workspaceService.GetWorkspaceIfUserCanAccessAsync(request.WorkspaceId, cancellationToken);
        var project = await _projectService.GetProjectIfUserCanAccessAsync(
            workspaceId: workspace.Id,
            projectId: request.ProjectId,
            includeWikiSections: true,
            cancellationToken: cancellationToken);

        var section = project.WikiSections.FirstOrDefault(section => section.Id.Equals(request.WikiSectionId))
            ?? throw new QuillityException(StatusCodes.NOT_FOUND, "Wiki section not found");

        return new WikiSectionDTO
        {
            Id = section.Id,
            Name = section.Name,
            Description = section.Description,
            IsDeprecated = section.IsDeprecated,
            IsLocked = section.IsLocked,
        };
    }
}