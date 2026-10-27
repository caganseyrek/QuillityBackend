using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.Members.Queries;

public class GetProjectMemberByIdQuery : IRequest<ProjectMemberDTO>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
    public Guid MemberId { get; set; }
}

public class GetProjectMemberByIdQueryHandler : IRequestHandler<GetProjectMemberByIdQuery, ProjectMemberDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;

    public GetProjectMemberByIdQueryHandler(QuillityDbContext dbContext, PermissionService permissionService)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
    }

    public async Task<ProjectMemberDTO> Handle(GetProjectMemberByIdQuery request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.None,
            cancellationToken);

        var project = await _permissionService.AuthorizeProjectAccessAsync(
            workspaceId: workspace.Id,
            projectId: request.ProjectId,
            requiredPermission: ProjectPermissions.None,
            cancellationToken);

        var projectMember = await _dbContext.ProjectMembers
            .FirstOrDefaultAsync(user =>
                StringHelper.IsEqualIgnoreCase(user.Id, request.MemberId) &&
                StringHelper.IsEqualIgnoreCase(user.ProjectId, workspace.Id), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.ProjectMember));

        return ObjectMapper.ToProjectMemberDTO(projectMember);
    }
}
