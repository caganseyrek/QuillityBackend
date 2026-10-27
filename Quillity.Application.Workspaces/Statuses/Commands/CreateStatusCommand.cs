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

namespace Quillity.Application.Workspaces.Statuses.Commands;

public class CreateStatusCommandBody
{
    public required string Name { get; set; }
    public required string ColorCode { get; set; }
    public StatusItemGroup Group { get; set; }
}

public class CreateStatusCommand : CreateStatusCommandBody, IRequest<StatusDTO>
{
    public Guid WorkspaceId { get; set; }
}

public class CreateStatusCommandHandler : IRequestHandler<CreateStatusCommand, StatusDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public CreateStatusCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<StatusDTO> Handle(CreateStatusCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.CreateStatus,
            cancellationToken);

        var statusWithSameName = await _dbContext.Statuses
            .FirstOrDefaultAsync(status =>
                StringHelper.IsEqualIgnoreCase(status.WorkspaceId, workspace.Id) &&
                StringHelper.IsEqualIgnoreCase(status.Name, request.Name), cancellationToken);
        if (statusWithSameName is not null)
            throw new QuillityException(
                StatusCodes.BAD_REQUEST,
                ErrorMessages.GetEntryWithSamePropExistsError(ErrorMessageModel.Status, "name", ErrorMessageScope.Workspace));

        var newStatus = new StatusModel
        {
            WorkspaceId = workspace.Id,
            Workspace = workspace,
            Name = request.Name,
            ColorCode = request.ColorCode.ToUpper(),
            Group = request.Group,
        };

        await _dbContext.Statuses.AddAsync(newStatus, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToStatusDTO(newStatus);
    }
}
