using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Common.Context;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Infrastructure;

namespace Quillity.Application.User.Core.Queries;

public class GetUserDetailsQuery : IRequest<UserDetailsDTO>
{
}

public class GetUserDetailsQueryHandler : IRequestHandler<GetUserDetailsQuery, UserDetailsDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly UserContext _userContext;

    public GetUserDetailsQueryHandler(QuillityDbContext dbContext, UserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<UserDetailsDTO> Handle(GetUserDetailsQuery request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(user => StringHelper.IsEqualIgnoreCase(user.Id, _userContext.User.Id), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.User));

        return ObjectMapper.ToUserDetailsDTO(user);
    }
}
