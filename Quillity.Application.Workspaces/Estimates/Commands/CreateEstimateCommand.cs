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

namespace Quillity.Application.Workspaces.Estimates.Commands;

public class CreateEstimateCommandBody 
{
    public required string Name { get; set; }
}

public class CreateEstimateCommand : CreateEstimateCommandBody, IRequest<EstimateDTO>
{
    public Guid WorkspaceId { get; set; }
}

public class CreateEstimateCommandHandler : IRequestHandler<CreateEstimateCommand, EstimateDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public CreateEstimateCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<EstimateDTO> Handle(CreateEstimateCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.CreateEstimate,
            cancellationToken);

        var estimateWithSameName = await _dbContext.Estimates
            .AnyAsync(estimate =>
                StringHelper.IsEqualIgnoreCase(estimate.WorkspaceId, workspace.Id) &&
                StringHelper.IsEqualIgnoreCase(estimate.Name, request.Name), cancellationToken);
        if (estimateWithSameName)
            throw new QuillityException(
                StatusCodes.BAD_REQUEST,
                ErrorMessages.GetEntryWithSamePropExistsError(ErrorMessageModel.Estimate, "name", ErrorMessageScope.Workspace));

        var newEstimate = new EstimateModel
        {
            Workspace = workspace,
            WorkspaceId = workspace.Id,
            Name = request.Name,
        };

        await _dbContext.Estimates.AddAsync(newEstimate, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToEstimateDTO(newEstimate);
    }
}
