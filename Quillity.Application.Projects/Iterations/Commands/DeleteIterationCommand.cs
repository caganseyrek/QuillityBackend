using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.Iterations.Commands;

public class DeleteIterationCommand : IRequest<Unit>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid IterationId { get; set; }
}

public class DeleteIterationCommandHandler : IRequestHandler<DeleteIterationCommand, Unit>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public DeleteIterationCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<Unit> Handle(DeleteIterationCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.None,
            cancellationToken);

        var project = await _permissionService.AuthorizeProjectAccessAsync(
            workspaceId: workspace.Id,
            projectId: request.ProjectId,
            requiredPermission: ProjectPermissions.DeleteIteration,
            cancellationToken);

        var existingIteration = await _dbContext.Iterations
            .FirstOrDefaultAsync(iteration =>
                StringHelper.IsEqualIgnoreCase(iteration.Id, request.IterationId) &&
                StringHelper.IsEqualIgnoreCase(iteration.ProjectId, project.Id), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.Iteration));

        _dbContext.Iterations.Remove(existingIteration);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
