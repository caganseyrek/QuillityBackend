using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Auth;
using Quillity.Common.Context;
using Quillity.Common.Helpers;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.Core.Queries;

public class GetProjectsQuery : IRequest<List<ProjectDTO>>
{
    public Guid WorkspaceId { get; set; }
}

public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, List<ProjectDTO>>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;
    private readonly UserContext _userContext;

    public GetProjectsQueryHandler(
        QuillityDbContext dbContext,
        PermissionService permissionService,
        UserContext userContext)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
        _userContext = userContext;
    }

    public async Task<List<ProjectDTO>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.None,
            cancellationToken);

        var projects = _dbContext.Projects
            .Include(project => project.Members)
                .ThenInclude(member => member.Role)
            .Where(project =>
                StringHelper.IsEqualIgnoreCase(project.WorkspaceId, workspace.Id) &&
                project.Members.Any(member => StringHelper.IsEqualIgnoreCase(member.UserId, _userContext.User.Id)))
            .Select(ObjectMapper.ToProjectDTO)
            .ToList();

        return projects;
    }
}
