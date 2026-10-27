using MediatR;
using Quillity.Application.DTOs;
using Quillity.Common.Context;
using Quillity.Common.Helpers;
using Quillity.Infrastructure;

namespace Quillity.Application.User.Core.Queries;

public class GetInvitationsQuery : IRequest<List<InvitationDTO>>
{
}

public class GetInvitationsQueryHandler : IRequestHandler<GetInvitationsQuery, List<InvitationDTO>>
{
    private readonly QuillityDbContext _dbContext;
    private readonly UserContext _userContext;

    public GetInvitationsQueryHandler(QuillityDbContext dbContext, UserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public Task<List<InvitationDTO>> Handle(GetInvitationsQuery request, CancellationToken cancellationToken)
    {
        var invitationDTOs = _dbContext.Invitations
            .Where(invitation => StringHelper.IsEqualIgnoreCase(invitation.InvitedUserId, _userContext.User.Id))
            .Select(ObjectMapper.ToInvitationDTO)
            .ToList();

        return Task.FromResult(invitationDTOs);
    }
}
