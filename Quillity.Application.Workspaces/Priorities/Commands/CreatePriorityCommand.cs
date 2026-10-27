using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Models;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Priorities.Commands;

public class CreatePriorityCommandBody
{
    public required string Name { get; set; }
    public required string ColorCode { get; set; }
    public int MagnitudeIndex { get; set; }
}

public class CreatePriorityCommand : CreatePriorityCommandBody, IRequest<PriorityDTO>
{
    public required Guid WorkspaceId { get; set; }
}

public class CreatePriorityCommandHandler : IRequestHandler<CreatePriorityCommand, PriorityDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public CreatePriorityCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<PriorityDTO> Handle(CreatePriorityCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.CreatePriority,
            cancellationToken);

        var priorityWithSameName = await _dbContext.Priorities
            .AnyAsync(label =>
                StringHelper.IsEqualIgnoreCase(label.WorkspaceId, workspace.Id) &&
                StringHelper.IsEqualIgnoreCase(label.Name, request.Name), cancellationToken);
        if (priorityWithSameName)
            throw new QuillityException(
                StatusCodes.BAD_REQUEST,
                ErrorMessages.GetEntryWithSamePropExistsError(ErrorMessageModel.Priority, "name", ErrorMessageScope.Workspace));

        var newPriority = new PriorityModel
        {
            WorkspaceId = workspace.Id,
            Workspace = workspace,
            Name = request.Name,
            ColorCode = request.ColorCode.ToUpper(),
            MagnitudeIndex = request.MagnitudeIndex,
        };

        await _dbContext.Priorities.AddAsync(newPriority, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToPriorityDTO(newPriority);
    }
}
