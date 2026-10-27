using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Common.Context;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Infrastructure;

namespace Quillity.Application.User.Core.Commands;

public class UpdateUserPreferencesCommand : IRequest<UserPreferencesDTO>
{
    public required string PreferredUiTheme { get; set; }
    public required string UserTimeZone { get; set; }
    public required string FirstDayOfWeek { get; set; }
    public required string DateFormat { get; set; }
    public required string TimeFormat { get; set; }
    public bool Use24HourClock { get; set; }
    public bool AutoAssignNewTasksToSelf { get; set; }
}

public class UpdateUserPreferencesCommandHandler : IRequestHandler<UpdateUserPreferencesCommand, UserPreferencesDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly UserContext _userContext;

    public UpdateUserPreferencesCommandHandler(QuillityDbContext dbContext, UserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<UserPreferencesDTO> Handle(UpdateUserPreferencesCommand request, CancellationToken cancellationToken)
    {
        var existingUserPreferences = await _dbContext.UserPreferences
            .FirstOrDefaultAsync(preferences => StringHelper.IsEqualIgnoreCase(preferences.UserId, _userContext.User.Id), cancellationToken)
            ?? throw new QuillityException(StatusCodes.UNAUTHORIZED, "You are not authorized to update preferences for this user.");

        existingUserPreferences.PreferredUiTheme = request.PreferredUiTheme;
        existingUserPreferences.UserTimeZone = request.UserTimeZone;
        existingUserPreferences.FirstDayOfWeek = request.FirstDayOfWeek;
        existingUserPreferences.DateFormat = request.DateFormat;
        existingUserPreferences.TimeFormat = request.TimeFormat;
        existingUserPreferences.Use24HourClock = request.Use24HourClock;
        existingUserPreferences.AutoAssignNewTasksToSelf = request.AutoAssignNewTasksToSelf;

        _dbContext.UserPreferences.Update(existingUserPreferences);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToUserPreferencesDTO(existingUserPreferences);
    }
}
