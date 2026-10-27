using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Priorities.Commands;

public class UpdatePriorityCommandBody
{
    public required string Name { get; set; }
    public required string ColorCode { get; set; }
    public int MagnitudeIndex { get; set; }
}

public class UpdatePriorityCommand : UpdatePriorityCommandBody, IRequest<PriorityDTO>
{
    public required Guid WorkspaceId { get; set; }
    public required Guid PriorityId { get; set; }
}

public class UpdatePriorityCommandHandler : IRequestHandler<UpdatePriorityCommand, PriorityDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public UpdatePriorityCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<PriorityDTO> Handle(UpdatePriorityCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.UpdatePriority,
            cancellationToken);

        var priorityWithSameName = await _dbContext.Priorities
            .AnyAsync(label =>
                !StringHelper.IsEqualIgnoreCase(label.Id, request.PriorityId) &&
                StringHelper.IsEqualIgnoreCase(label.WorkspaceId, workspace.Id) &&
                StringHelper.IsEqualIgnoreCase(label.Name, request.Name), cancellationToken);
        if (priorityWithSameName)
            throw new QuillityException(
                StatusCodes.BAD_REQUEST,
                ErrorMessages.GetEntryWithSamePropExistsError(ErrorMessageModel.Label, "name", ErrorMessageScope.Workspace));

        var existingPriority = await _dbContext.Priorities
            .FirstOrDefaultAsync(priority =>
                StringHelper.IsEqualIgnoreCase(priority.Id, request.PriorityId) &&
                StringHelper.IsEqualIgnoreCase(priority.WorkspaceId, workspace.Id), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.Priority));

        existingPriority.Name = request.Name;
        existingPriority.MagnitudeIndex = request.MagnitudeIndex;
        existingPriority.ColorCode = request.ColorCode.ToUpper();

        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToPriorityDTO(existingPriority);
    }
}
