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

namespace Quillity.Application.Workspaces.Labels.Commands;

public class CreateLabelCommandBody
{
    public required string Name { get; set; }
    public required string ColorCode { get; set; }
    public required string Description { get; set; }
}

public class CreateLabelCommand : CreateLabelCommandBody, IRequest<LabelDTO>
{
    public Guid WorkspaceId { get; set; }
}

public class CreateLabelCommandHandler : IRequestHandler<CreateLabelCommand, LabelDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public CreateLabelCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<LabelDTO> Handle(CreateLabelCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.CreateLabel,
            cancellationToken);

        var labelWithSameName = await _dbContext.Labels
            .AnyAsync(label =>
                StringHelper.IsEqualIgnoreCase(label.WorkspaceId, workspace.Id) &&
                StringHelper.IsEqualIgnoreCase(label.Name, request.Name), cancellationToken);
        if (labelWithSameName)
            throw new QuillityException(
                StatusCodes.BAD_REQUEST,
                ErrorMessages.GetEntryWithSamePropExistsError(ErrorMessageModel.Label, "name", ErrorMessageScope.Workspace));

        var newLabel = new LabelModel
        {
            WorkspaceId = request.WorkspaceId,
            Workspace = workspace,
            Name = request.Name,
            ColorCode = request.ColorCode.ToUpper(),
            Description = StringHelper.NullIfWhiteSpace(request.Description),
        };

        await _dbContext.Labels.AddAsync(newLabel, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToLabelDTO(newLabel);
    }
}
