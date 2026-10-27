namespace Quillity.Common.Values;

public enum HealthUpdateStatus
{
    OnTrack = 0,
    AtRisk = 1,
    OffTrack = 2,
}

public enum StatusItemGroup
{
    Backlog = 0,
    NotStarted = 1,
    InProgress = 2,
    Completed = 3,
    Canceled = 4,
}

public enum ActivityLogType
{
    Info = 0,
    Warning = 1,
    Error = 2,
    System = 3,
}

public enum ActivityLogVisibility
{
    WorkspacePublic = 0,
    WorkspaceLeadOnly = 1,
    UserPersonal = 2,
    SystemInternal = 3,
}
