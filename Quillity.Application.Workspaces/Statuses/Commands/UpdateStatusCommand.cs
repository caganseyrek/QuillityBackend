using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Statuses.Commands;

public class UpdateStatusCommandBody
{
    public required string Name { get; set; }
    public required string ColorCode { get; set; }
    public StatusItemGroup Group { get; set; }
}

public class UpdateStatusCommand : UpdateStatusCommandBody, IRequest<StatusDTO>
{
    public Guid WorkspaceId { get; set; }
    public Guid StatusId { get; set; }
}

public class UpdateStatusCommandHandler : IRequestHandler<UpdateStatusCommand, StatusDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public UpdateStatusCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<StatusDTO> Handle(UpdateStatusCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.UpdateStatus,
            cancellationToken);

        var statusWithSameName = await _dbContext.Statuses
            .AnyAsync(status =>
                !StringHelper.IsEqualIgnoreCase(status.Id, request.StatusId) &&
                StringHelper.IsEqualIgnoreCase(status.WorkspaceId, workspace.Id) &&
                StringHelper.IsEqualIgnoreCase(status.Name, request.Name), cancellationToken);
        if (statusWithSameName)
            throw new QuillityException(
                StatusCodes.BAD_REQUEST,
                ErrorMessages.GetEntryWithSamePropExistsError(ErrorMessageModel.Status, "name", ErrorMessageScope.Workspace));

        var existingStatus = await _dbContext.Statuses
            .FirstOrDefaultAsync(status =>
                StringHelper.IsEqualIgnoreCase(status.Id, request.StatusId) &&
                StringHelper.IsEqualIgnoreCase(status.WorkspaceId, workspace.Id), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.Status));

        if (existingStatus.Group != request.Group)
        {
            var hasOtherStatusesInGroup = await _dbContext.Statuses
                .AnyAsync(status =>
                    !StringHelper.IsEqualIgnoreCase(status.Id, request.StatusId) &&
                    StringHelper.IsEqualIgnoreCase(status.WorkspaceId, workspace.Id) &&
                    status.Group == request.Group, cancellationToken);
            if (!hasOtherStatusesInGroup)
                throw new QuillityException(
                    StatusCodes.BAD_REQUEST,
                    "Each status group should have at least one status.");

            existingStatus.Group = request.Group;
        }

        existingStatus.Name = request.Name;
        existingStatus.ColorCode = request.ColorCode.ToUpper();

        _dbContext.Statuses.Update(existingStatus);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToStatusDTO(existingStatus);
    }
}
