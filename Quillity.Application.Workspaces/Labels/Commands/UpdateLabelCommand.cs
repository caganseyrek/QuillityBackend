using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Labels.Commands;

public class UpdateLabelCommandBody
{
    public required string Name { get; set; }
    public required string ColorCode { get; set; }
    public required string Description { get; set; }
}

public class UpdateLabelCommand : UpdateLabelCommandBody, IRequest<LabelDTO>
{
    public Guid WorkspaceId { get; set; }
    public Guid LabelId { get; set; }
}

public class UpdateLabelCommandHandler : IRequestHandler<UpdateLabelCommand, LabelDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public UpdateLabelCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<LabelDTO> Handle(UpdateLabelCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.UpdateLabel,
            cancellationToken);

        var labelWithSameName = await _dbContext.Labels
            .AnyAsync(label =>
                StringHelper.IsEqualIgnoreCase(label.WorkspaceId, workspace.Id) &&
                !StringHelper.IsEqualIgnoreCase(label.Id, request.LabelId) &&
                StringHelper.IsEqualIgnoreCase(label.Name, request.Name), cancellationToken);
        if (labelWithSameName)
            throw new QuillityException(
                StatusCodes.BAD_REQUEST,
                ErrorMessages.GetEntryWithSamePropExistsError(ErrorMessageModel.Label, "name", ErrorMessageScope.Workspace));

        var existingLabel = await _dbContext.Labels
            .FirstOrDefaultAsync(label =>
                StringHelper.IsEqualIgnoreCase(label.WorkspaceId, workspace.Id) &&
                StringHelper.IsEqualIgnoreCase(label.Id, request.LabelId), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.Label));

        existingLabel.Name = request.Name;
        existingLabel.Description = StringHelper.NullIfWhiteSpace(request.Description);
        existingLabel.ColorCode = request.ColorCode.ToUpper();

        _dbContext.Labels.Update(existingLabel);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToLabelDTO(existingLabel);
    }
}
