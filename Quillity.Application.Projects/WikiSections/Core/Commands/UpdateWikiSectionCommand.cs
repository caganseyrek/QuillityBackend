using MediatR;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Response;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.WikiSections.Core.Commands;

public class UpdateWikiSectionCommandBody
{
    public required string Name { get; set; }
    public required string Description { get; set; }
}

public class UpdateWikiSectionCommand : UpdateWikiSectionCommandBody, IRequest<Unit>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid WikiSectionId { get; set; }
}

public class UpdateWikiSectionCommandHandler : IRequestHandler<UpdateWikiSectionCommand, Unit>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _workspaceService;
    private readonly ProjectService _projectService;

    public UpdateWikiSectionCommandHandler(
        QuillityDbContext dbContext,
        PermissionService workspaceService,
        ProjectService projectService)
    {
        _dbContext = dbContext;
        _workspaceService = workspaceService;
        _projectService = projectService;
    }

    public async Task<Unit> Handle(UpdateWikiSectionCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _workspaceService.GetWorkspaceIfUserCanAccessAsync(request.WorkspaceId, cancellationToken);
        var project = await _projectService.GetProjectIfUserCanAccessAsync(
            workspaceId: workspace.Id,
            projectId: request.ProjectId,
            includeWikiSections: true,
            cancellationToken: cancellationToken);

        if (!_projectService.CanUserManageWikiPages(project))
            throw new QuillityException(StatusCodes.UNAUTHORIZED, "You do not have permission to manage wiki pages in this project.");

        var section = project.WikiSections.FirstOrDefault(section => section.Id.Equals(request.WikiSectionId) && section.ProjectId.Equals(project.Id))
            ?? throw new QuillityException(StatusCodes.NOT_FOUND, "Wiki section not found.");
        
        var wikiSectionWithSameName = project.WikiSections.FirstOrDefault(wikiSection => wikiSection.Name.Equals(request.Name) && !wikiSection.Id.Equals(section.Id));
        if (wikiSectionWithSameName is not null)
            throw new QuillityException(StatusCodes.BAD_REQUEST, "A section with the same name already exists in this project wiki.");

        section.Name = request.Name;
        section.Description = request.Description;

        _dbContext.WikiSections.Update(section);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}