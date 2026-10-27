using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Core.Commands;

public class UpdateWorkspaceCommandBody
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string UrlKey { get; set; }
    public required string LogoImageUrl { get; set; }
}

public class UpdateWorkspaceCommand : UpdateWorkspaceCommandBody, IRequest<WorkspaceDTO>
{
    public Guid WorkspaceId { get; set; }
}

public class UpdateWorkspaceCommandHandler : IRequestHandler<UpdateWorkspaceCommand, WorkspaceDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public UpdateWorkspaceCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<WorkspaceDTO> Handle(UpdateWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.UpdateWorkspace,
            cancellationToken);

        var workspaceWithSameUrlKey = await _dbContext.Workspaces
            .AnyAsync(workspace =>
                !StringHelper.IsEqualIgnoreCase(workspace.Id, workspace.Id) &&
                StringHelper.IsEqualIgnoreCase(workspace.UrlKey, request.UrlKey), cancellationToken);
        if (workspaceWithSameUrlKey is true)
            throw new QuillityException(
                StatusCodes.BAD_REQUEST,
                ErrorMessages.GetEntryWithSamePropExistsError(ErrorMessageModel.Workspace, "url key"));

        workspace.Name = request.Name;
        workspace.Description = request.Description;
        workspace.UrlKey = request.UrlKey;
        workspace.LogoImageUrl = StringHelper.NullIfWhiteSpace(request.LogoImageUrl);
        workspace.InitialsForFallback = StringHelper.GetInitialLetters(request.Name);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToWorkspaceDTO(workspace);
    }
}
