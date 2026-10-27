using MediatR;
using Quillity.Auth;
using Quillity.Common.Context;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Projects.Members.Commands;

public class ChangeProjectLeadCommandBody
{
    public Guid NewLeadMemberId { get; set; }
}

public class ChangeProjectLeadCommand : ChangeProjectLeadCommandBody, IRequest<Unit>
{
    public Guid WorkspaceId { get; set; }
    public Guid ProjectId { get; set; }
}

public class ChangeProjectLeadCommandHandler : IRequestHandler<ChangeProjectLeadCommand, Unit>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PermissionService _permissionService;
    private readonly UserContext _userContext;

    public ChangeProjectLeadCommandHandler(
        QuillityDbContext dbContext,
        PermissionService permissionService,
        UserContext userContext)
    {
        _dbContext = dbContext;
        _permissionService = permissionService;
        _userContext = userContext;
    }

    public async Task<Unit> Handle(ChangeProjectLeadCommand request, CancellationToken cancellationToken)
    {
        var workspace = await _permissionService.AuthorizeWorkspaceAccessAsync(
            workspaceId: request.WorkspaceId,
            requiredPermission: WorkspacePermissions.ChangeAnyProjectLead,
            cancellationToken);

        var project = await _permissionService.AuthorizeProjectAccessAsync(
            workspaceId: workspace.Id,
            projectId: request.ProjectId,
            requiredPermission: ProjectPermissions.None,
            cancellationToken);

        var currentUserMember = project.Members
            .FirstOrDefault(member => StringHelper.IsEqualIgnoreCase(member.UserId, _userContext.User.Id))
            ?? throw new QuillityException(
                StatusCodes.FORBIDDEN,
                ErrorMessages.GetForbiddenError());

        if (StringHelper.IsEqualIgnoreCase(currentUserMember.RoleId, project.LeadRoleId))
            throw new QuillityException(
                StatusCodes.FORBIDDEN,
                ErrorMessages.GetForbiddenError());

        var newUserMember = project.Members
            .FirstOrDefault(member => StringHelper.IsEqualIgnoreCase(member.Id, request.NewLeadMemberId))
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.ProjectMember));

        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            currentUserMember.Role = project.DefaultRole!;
            currentUserMember.RoleId = project.DefaultRoleId;

            newUserMember.Role = project.LeadRole!;
            newUserMember.RoleId = project.LeadRoleId;

            _dbContext.ProjectMembers.Update(currentUserMember);
            _dbContext.ProjectMembers.Update(newUserMember);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new QuillityException(StatusCodes.SERVER_ERROR, ErrorMessages.GetServerError());
        }
        return Unit.Value;
    }
}
