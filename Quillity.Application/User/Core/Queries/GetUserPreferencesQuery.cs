using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Common.Context;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Infrastructure;

namespace Quillity.Application.User.Core.Queries;

public class GetUserPreferencesQuery : IRequest<UserPreferencesDTO>
{
}

public class GetUserPreferencesQueryHandler : IRequestHandler<GetUserPreferencesQuery, UserPreferencesDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly UserContext _userContext;

    public GetUserPreferencesQueryHandler(QuillityDbContext dbContext, UserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<UserPreferencesDTO> Handle(GetUserPreferencesQuery request, CancellationToken cancellationToken)
    {
        var userPreferences = await _dbContext.UserPreferences
            .FirstOrDefaultAsync(preferences => StringHelper.IsEqualIgnoreCase(preferences.UserId, _userContext.User.Id), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.User));

        return ObjectMapper.ToUserPreferencesDTO(userPreferences);
    }
}
