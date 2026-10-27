using MediatR;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Context;
using Quillity.Common.Helpers;
using Quillity.Common.Models;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.HealthUpdates.Core.Commands;

public class CreateHealthUpdateCommandBody
{
    public required string Summary { get; set; }
    public HealthUpdateStatus HealthStatus { get; set; }
}

public class CreateHealthUpdateCommand : CreateHealthUpdateCommandBody, IRequest<HealthUpdateDTO>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
}

public class CreateHealthUpdateCommandHandler : IRequestHandler<CreateHealthUpdateCommand, HealthUpdateDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;
    private readonly UserContext _userContext;

    public CreateHealthUpdateCommandHandler(
        QuillityDbContext dbContext,
        PermissionService permissionService,
        UserContext userContext)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
        _userContext = userContext;
    }

    public async Task<HealthUpdateDTO> Handle(CreateHealthUpdateCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.None,
            cancellationToken);

        var project = await _permissionService.AuthorizeProjectAccessAsync(
            workspaceId: workspace.Id,
            projectId: request.ProjectId,
            requiredPermission: ProjectPermissions.CreateHealthUpdate,
            cancellationToken);

        var userProjectMembership = project.Members
            .FirstOrDefault(member => StringHelper.IsEqualIgnoreCase(member.User.Id, _userContext.User.Id))
            ?? throw new Exception(ErrorMessages.GetProjectAccessLogicError());

        var newHealthUpdate = new HealthUpdateModel
        {
            Project = project,
            ProjectId = project.Id,
            Author = userProjectMembership,
            AuthorId = userProjectMembership.Id,
            Status = request.HealthStatus,
            Summary = request.Summary,
            PublishedAt = DateTimeOffset.Now,
        };

        await _dbContext.HealthUpdates.AddAsync(newHealthUpdate, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToHealthUpdateDTO(newHealthUpdate);
    }
}