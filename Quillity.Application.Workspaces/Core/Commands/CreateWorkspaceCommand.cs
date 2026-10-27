using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Common.Context;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Models;
using Quillity.Common.Presets;
using Quillity.Common.Response;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Core.Commands;

public class CreateWorkspaceCommand : IRequest<WorkspaceDTO>
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required string UrlKey { get; set; }
    public required string LogoImageUrl { get; set; }
}

public class CreateWorkspaceCommandHandler : IRequestHandler<CreateWorkspaceCommand, WorkspaceDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly UserContext _userContext;

    public CreateWorkspaceCommandHandler(
        QuillityDbContext dbContext,
        UserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<WorkspaceDTO> Handle(CreateWorkspaceCommand request, CancellationToken cancellationToken)
    {
        // Does the url key already exist in the database?
        var urlKeyExists = await _dbContext.Workspaces
            .AnyAsync(workspace => StringHelper.IsEqualIgnoreCase(workspace.UrlKey, request.UrlKey), cancellationToken);
        if (urlKeyExists)
            throw new QuillityException(
                StatusCodes.BAD_REQUEST,
                ErrorMessages.GetEntryWithSamePropExistsError(ErrorMessageModel.Workspace, "url key"));

        var newWorkspace = new WorkspaceModel
        {
            Name = request.Name,
            Description = request.Description,
            UrlKey = request.UrlKey,
            LogoImageUrl = StringHelper.NullIfWhiteSpace(request.LogoImageUrl),
            InitialsForFallback = StringHelper.GetInitialLetters(request.Name),
        };

        var workspacePresets = new WorkspacePresets(newWorkspace);

        var ownerRole = workspacePresets.GetOwnerRole();
        newWorkspace.OwnerRoleId = ownerRole.Id;
        newWorkspace.OwnerRole = ownerRole;

        var defaultRole = workspacePresets.GetMemberRole();
        newWorkspace.DefaultRoleId = defaultRole.Id;
        newWorkspace.DefaultRole = defaultRole;

        var roles = new List<WorkspaceRoleModel>
        {
            ownerRole,
            workspacePresets.GetAdminRole(),
            defaultRole,
        };

        var workspaceOwnerMember = new WorkspaceMemberModel
        {
            Workspace = newWorkspace,
            WorkspaceId = newWorkspace.Id,
            User = _userContext.User,
            UserId = _userContext.User.Id,
            Role = ownerRole,
            RoleId = ownerRole.Id,
        };

        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await _dbContext.Workspaces.AddAsync(newWorkspace, cancellationToken);
            await _dbContext.WorkspaceMembers.AddAsync(workspaceOwnerMember, cancellationToken);
            await _dbContext.WorkspaceRoles.AddRangeAsync(roles, cancellationToken);

            await _dbContext.Statuses.AddRangeAsync(workspacePresets.GetStatuses(), cancellationToken);
            await _dbContext.Priorities.AddRangeAsync(workspacePresets.GetPriorities(), cancellationToken);
            await _dbContext.Estimates.AddRangeAsync(workspacePresets.GetEstimates(), cancellationToken);
            await _dbContext.Labels.AddRangeAsync(workspacePresets.GetLabels(), cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new QuillityException(StatusCodes.SERVER_ERROR, ErrorMessages.GetServerError());
        }

        return ObjectMapper.ToWorkspaceDTO(newWorkspace);
    }
}
