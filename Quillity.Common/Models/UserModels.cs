namespace Quillity.Common.Models;

public class UserModel : ModelBase
{
    public required string FullName { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public string? EmailVerificationToken { get; set; } = null;
    public long? EmailVerificationTokenExpirationInMs { get; set; } = null;
    public bool EmailVerified { get; set; } = false;
    public string PasswordHash { get; set; } = string.Empty;
    public string? PasswordResetToken { get; set; } = null;
    public long? PasswordResetTokenExpirationInMs { get; set; } = null;
    public string? ProfilePictureUrl { get; set; } = null;
    public required string InitialsForFallback { get; set; }
    public bool IsSuspended { get; set; } =  false;
}

public class UserPreferencesModel : ModelBase
{
    public Guid UserId { get; set; }
    public required UserModel User { get; set; }
    public required string PreferredUiTheme { get; set; }
    public required string UserTimeZone { get; set; }
    public required string FirstDayOfWeek { get; set; }
    public required string DateFormat { get; set; }
    public required string TimeFormat { get; set; }
    public bool Use24HourClock { get; set; } = true;
    public bool AutoAssignNewTasksToSelf { get; set; } = false;
}

public class InvitationModel : ModelBase
{
    public Guid InvitedWorkspaceId { get; set; }
    public required WorkspaceModel InvitedWorkspace { get; set; }
    public Guid InvitedByUserId { get; set; }
    public required UserModel InvitedByUser { get; set; }
    public Guid InvitedUserId { get; set; }
    public required UserModel InvitedUser { get; set; }
    public required string InvitationNote { get; set; }
    public long InvitedAtMs { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    public long? AcceptedAtMs { get; set; } = null;
}

public class NotepadPageModel : PageModelBase
{
    public Guid OwnerId { get; set; }
    public required UserModel Owner { get; set; }
}
