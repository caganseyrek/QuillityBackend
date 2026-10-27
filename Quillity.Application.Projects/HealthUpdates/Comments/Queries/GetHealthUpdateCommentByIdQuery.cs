using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.HealthUpdates.Comments.Queries;

public class GetHealthUpdateCommentByIdQuery : IRequest<HealthUpdateCommentDTO>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid HealthUpdateId { get; set; }
}

public class GetHealthUpdateCommentByIdQueryHandler : IRequestHandler<GetHealthUpdateCommentByIdQuery, HealthUpdateCommentDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public GetHealthUpdateCommentByIdQueryHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<HealthUpdateCommentDTO> Handle(GetHealthUpdateCommentByIdQuery request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.None,
            cancellationToken);

        var project = await _permissionService.AuthorizeProjectAccessAsync(
            workspaceId: workspace.Id,
            projectId: request.ProjectId,
            requiredPermission: ProjectPermissions.None,
            cancellationToken);

        var existingHealthUpdateComment = await _dbContext.HealthUpdateComments
            .FirstOrDefaultAsync(update =>
                StringHelper.IsEqualIgnoreCase(update.HealthUpdateId, request.HealthUpdateId) &&
                StringHelper.IsEqualIgnoreCase(update.Id, request.HealthUpdateId), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.HealthUpdateComment));

        return ObjectMapper.ToHealthUpdateCommentDTO(existingHealthUpdateComment);
    }
}
