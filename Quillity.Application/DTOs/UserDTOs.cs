namespace Quillity.Application.DTOs;

public class UserDetailsDTO : DTOBase
{
    public required string FullName { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public required string InitialsForFallback { get; set; }
}

public class UserPreferencesDTO : DTOBase
{
    public required string PreferredUITheme { get; set; }
    public required string UserTimeZone { get; set; }
    public required string FirstDayOfWeek { get; set; }
    public required string DateFormat { get; set; }
    public required string TimeFormat { get; set; }
    public bool Use24HourClock { get; set; } = true;
    public bool AutoAssignNewTasksToSelf { get; set; }
}

public class InvitationDTO : DTOBase
{
    public required WorkspaceDTO InvitedWorkspace { get; set; }
    public required UserDetailsDTO InvitedUser { get; set; }
    public required UserDetailsDTO InvitedByUser { get; set; }
    public required string InvitationNote { get; set; }
    public long InvitedAtMs { get; set; }
    public long? AcceptedAtMs { get; set; } = null;
}

public class NotepadPageDTO : PageDTOBase { }
