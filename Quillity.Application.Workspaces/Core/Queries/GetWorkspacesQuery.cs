using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Common.Context;
using Quillity.Common.Helpers;
using Quillity.Infrastructure;

namespace Quillity.Application.Workspaces.Core.Queries;

public class GetWorkspacesQuery : IRequest<List<WorkspaceDTO>>
{
}

public class GetWorkspacesQueryHandler : IRequestHandler<GetWorkspacesQuery, List<WorkspaceDTO>>
{
    private readonly QuillityDbContext _dbContext;
    private readonly UserContext _userContext;

    public GetWorkspacesQueryHandler(QuillityDbContext dbContext, UserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<List<WorkspaceDTO>> Handle(GetWorkspacesQuery request, CancellationToken cancellationToken)
    {
        var workspaces = await _dbContext.Workspaces
            .Include(workspace => workspace.Members.Where(member =>
                StringHelper.IsEqualIgnoreCase(member.UserId, _userContext.User.Id)))
            .Where(workspace => workspace.Members.Any(member =>
                StringHelper.IsEqualIgnoreCase(member.UserId, _userContext.User.Id)))
            .ToListAsync(cancellationToken);

        return workspaces.Select(ObjectMapper.ToWorkspaceDTO).ToList();
    }
}