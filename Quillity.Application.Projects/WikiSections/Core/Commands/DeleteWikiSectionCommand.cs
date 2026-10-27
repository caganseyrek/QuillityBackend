using MediatR;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Response;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.WikiSections.Core.Commands;

public class DeleteWikiSectionCommand : IRequest<Unit>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid WikiSectionId { get; set; }
}

public class DeleteWikiSectionCommandHandler : IRequestHandler<DeleteWikiSectionCommand, Unit>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _workspaceService;
    private readonly ProjectService _projectService;

    public DeleteWikiSectionCommandHandler(
        QuillityDbContext dbContext,
        PermissionService workspaceService,
        ProjectService projectService)
    {
        _dbContext = dbContext;
        _workspaceService = workspaceService;
        _projectService = projectService;
    }

    public async Task<Unit> Handle(DeleteWikiSectionCommand request, CancellationToken cancellationToken)
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

        _dbContext.WikiSections.Remove(section);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}
