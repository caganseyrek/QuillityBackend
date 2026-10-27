using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Context;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Models;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.Members.Commands;

public class AddProjectMemberCommandBody
{
    public Guid NewMemberUserId { get; set; }
}

public class AddProjectMemberCommand : AddProjectMemberCommandBody, IRequest<ProjectMemberDTO>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
}

public class AddProjectMemberCommandHandler : IRequestHandler<AddProjectMemberCommand, ProjectMemberDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public AddProjectMemberCommandHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<ProjectMemberDTO> Handle(AddProjectMemberCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.None,
            cancellationToken);

        var project = await _permissionService.AuthorizeProjectAccessAsync(
            workspaceId: workspace.Id,
            projectId: request.ProjectId,
            requiredPermission: ProjectPermissions.AddProjectMember,
            cancellationToken);

        if (!project.Members.Any(member => StringHelper.IsEqualIgnoreCase(member.UserId, request.NewMemberUserId)))
            throw new QuillityException(StatusCodes.BAD_REQUEST, "User is already a member of this project.");

        var newUser = await _dbContext.Users.
            FirstOrDefaultAsync(user => StringHelper.IsEqualIgnoreCase(user.Id, request.NewMemberUserId), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.User));

        var newProjectMember = new ProjectMemberModel
        {
            ProjectId = project.Id,
            Project = project,
            UserId = newUser.Id,
            User = newUser,
            RoleId = project.DefaultRoleId,
            Role = project.DefaultRole!,
        };

        await _dbContext.ProjectMembers.AddAsync(newProjectMember, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToProjectMemberDTO(newProjectMember);
    }
}
