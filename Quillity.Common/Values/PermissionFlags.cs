namespace Quillity.Common.Values;

[Flags]
public enum WorkspacePermissions : long
{
    None = 0,

    // Core workspace permissions
    UpdateWorkspace = 1L << 0,
    DeleteWorkspace = 1L << 1,
    TransferWorkspaceOwnership = 1L << 2,
    ViewWorkspaceInvitations = 1L << 3,
    SendWorkspaceInvitation = 1L << 4,
    CancelWorkspaceInvitation = 1L << 5,
    RemoveWorkspaceMember = 1L << 6,
    ChangeWorkspaceMemberRole = 1L << 7,

    CreateWorkspaceRole = 1L << 8,
    UpdateWorkspaceRole = 1L << 9,
    DeleteWorkspaceRole = 1L << 10,

    // Estimate, priority, status, and label permissions
    CreateEstimate = 1L << 11,
    UpdateEstimate = 1L << 12,
    DeleteEstimate = 1L << 13,
    CreatePriority = 1L << 14,
    UpdatePriority = 1L << 15,
    DeletePriority = 1L << 16,
    CreateStatus = 1L << 17,
    UpdateStatus = 1L << 18,
    DeleteStatus = 1L << 19,
    CreateLabel = 1L << 20,
    UpdateLabel = 1L << 21,
    DeleteLabel = 1L << 22,

    // Project management permissions
    CreateProject = 1L << 23,
    ChangeAnyProjectLead = 1L << 24,

    AllValidWorkspacePermissions =
        UpdateWorkspace |
        DeleteWorkspace |
        TransferWorkspaceOwnership |
        ViewWorkspaceInvitations |
        SendWorkspaceInvitation |
        CancelWorkspaceInvitation |
        RemoveWorkspaceMember |
        ChangeWorkspaceMemberRole |
        CreateWorkspaceRole |
        UpdateWorkspaceRole |
        DeleteWorkspaceRole |
        CreateEstimate |
        UpdateEstimate |
        DeleteEstimate |
        CreatePriority |
        UpdatePriority |
        DeletePriority |
        CreateStatus |
        UpdateStatus |
        DeleteStatus |
        CreateLabel |
        UpdateLabel |
        DeleteLabel,
}

[Flags]
public enum ProjectPermissions : long
{
    None = 0,

    // Core project permissions
    UpdateProject = 1L << 0,
    DeleteProject = 1L << 1,
    AddProjectMember = 1L << 2,
    ChangeProjectMemberRole = 1L << 3,
    RemoveProjectMember = 1L << 4,
    ArchiveProject = 1L << 5,

    CreateHealthUpdate = 1L << 6,
    UpdateHealthUpdate = 1L << 7,
    DeleteHealthUpdate = 1L << 8,

    CreateMilestone = 1L << 9,
    UpdateMilestone = 1L << 10,
    DeleteMilestone = 1L << 11,

    CreateIteration = 1L << 12,
    UpdateIteration = 1L << 13,
    DeleteIteration = 1L << 14,

    CreateProjectRole = 1L << 15,
    UpdateProjectRole = 1L << 16,
    DeleteProjectRole = 1L << 17,

    DeleteAnyComment = 1L << 18,
    DeleteAnyReaction = 1L << 19,

    // Project wiki permissions
    CreateWikiSection = 1L << 20,
    UpdateWikiSection = 1L << 21,
    DeleteWikiSection = 1L << 22,
    CreateWikiSectionPage = 1L << 23,
    UpdateWikiSectionPage = 1L << 24,
    DeleteWikiSectionPage = 1L << 25,

    // Task item permissions
    CreateTaskItem = 1L << 26,
    UpdateTaskItem = 1L << 27,
    DeleteTaskItem = 1L << 28,
    AssignTaskItem = 1L << 29,
    CreateTaskItemRelation = 1L << 30,
    UpdateTaskItemRelation = 1L << 31,
    DeleteTaskItemRelation = 1L << 32,
    CreateTaskItemBlockage = 1L << 33,
    UpdateTaskItemBlockage = 1L << 34,
    DeleteTaskItemBlockage = 1L << 35,

    AllValidProjectPermissions =
        UpdateProject |
        DeleteProject |
        AddProjectMember |
        ChangeProjectMemberRole |
        RemoveProjectMember |
        ArchiveProject |
        CreateHealthUpdate |
        UpdateHealthUpdate |
        DeleteHealthUpdate |
        CreateMilestone |
        UpdateMilestone |
        DeleteMilestone |
        CreateIteration |
        UpdateIteration |
        DeleteIteration |
        CreateProjectRole |
        UpdateProjectRole |
        DeleteProjectRole |
        DeleteAnyComment |
        DeleteAnyReaction |
        CreateWikiSection |
        UpdateWikiSection |
        DeleteWikiSection |
        CreateWikiSectionPage |
        UpdateWikiSectionPage |
        DeleteWikiSectionPage |
        CreateTaskItem |
        UpdateTaskItem |
        DeleteTaskItem |
        AssignTaskItem |
        CreateTaskItemRelation |
        UpdateTaskItemRelation |
        DeleteTaskItemRelation |
        CreateTaskItemBlockage |
        UpdateTaskItemBlockage |
        DeleteTaskItemBlockage,
}

public static class PermissionsPerRole
{
    public static readonly WorkspacePermissions WorkspaceOwnerPermissions =
        WorkspacePermissions.UpdateWorkspace |
        WorkspacePermissions.DeleteWorkspace |
        WorkspacePermissions.TransferWorkspaceOwnership |
        WorkspacePermissions.ViewWorkspaceInvitations |
        WorkspacePermissions.SendWorkspaceInvitation |
        WorkspacePermissions.CancelWorkspaceInvitation |
        WorkspacePermissions.RemoveWorkspaceMember |
        WorkspacePermissions.ChangeWorkspaceMemberRole |
        WorkspacePermissions.CreateWorkspaceRole |
        WorkspacePermissions.UpdateWorkspaceRole |
        WorkspacePermissions.DeleteWorkspaceRole |
        WorkspacePermissions.CreateEstimate |
        WorkspacePermissions.UpdateEstimate |
        WorkspacePermissions.DeleteEstimate |
        WorkspacePermissions.CreatePriority |
        WorkspacePermissions.UpdatePriority |
        WorkspacePermissions.DeletePriority |
        WorkspacePermissions.CreateStatus |
        WorkspacePermissions.UpdateStatus |
        WorkspacePermissions.DeleteStatus |
        WorkspacePermissions.CreateLabel |
        WorkspacePermissions.UpdateLabel |
        WorkspacePermissions.DeleteLabel |
        WorkspacePermissions.CreateProject |
        WorkspacePermissions.ChangeAnyProjectLead;

    public static readonly WorkspacePermissions WorkspaceAdminPermissions =
        WorkspacePermissions.UpdateWorkspace |
        WorkspacePermissions.ViewWorkspaceInvitations |
        WorkspacePermissions.SendWorkspaceInvitation |
        WorkspacePermissions.CancelWorkspaceInvitation |
        WorkspacePermissions.RemoveWorkspaceMember |
        WorkspacePermissions.CreateEstimate |
        WorkspacePermissions.UpdateEstimate |
        WorkspacePermissions.DeleteEstimate |
        WorkspacePermissions.CreatePriority |
        WorkspacePermissions.UpdatePriority |
        WorkspacePermissions.DeletePriority |
        WorkspacePermissions.CreateStatus |
        WorkspacePermissions.UpdateStatus |
        WorkspacePermissions.DeleteStatus |
        WorkspacePermissions.CreateLabel |
        WorkspacePermissions.UpdateLabel |
        WorkspacePermissions.DeleteLabel |
        WorkspacePermissions.CreateProject;

    public static readonly WorkspacePermissions WorkspaceMemberPermissions = WorkspacePermissions.None;

    public static readonly ProjectPermissions ProjectLeadPermissions =
        ProjectPermissions.UpdateProject |
        ProjectPermissions.DeleteProject |
        ProjectPermissions.AddProjectMember |
        ProjectPermissions.ChangeProjectMemberRole |
        ProjectPermissions.RemoveProjectMember |
        ProjectPermissions.ArchiveProject |
        ProjectPermissions.CreateHealthUpdate |
        ProjectPermissions.UpdateHealthUpdate |
        ProjectPermissions.DeleteHealthUpdate |
        ProjectPermissions.CreateMilestone |
        ProjectPermissions.UpdateMilestone |
        ProjectPermissions.DeleteMilestone |
        ProjectPermissions.CreateIteration |
        ProjectPermissions.UpdateIteration |
        ProjectPermissions.DeleteIteration |
        ProjectPermissions.CreateProjectRole |
        ProjectPermissions.UpdateProjectRole |
        ProjectPermissions.DeleteProjectRole |
        ProjectPermissions.DeleteAnyComment |
        ProjectPermissions.DeleteAnyReaction |
        ProjectPermissions.CreateWikiSection |
        ProjectPermissions.UpdateWikiSection |
        ProjectPermissions.DeleteWikiSection |
        ProjectPermissions.CreateWikiSectionPage |
        ProjectPermissions.UpdateWikiSectionPage |
        ProjectPermissions.DeleteWikiSectionPage |
        ProjectPermissions.CreateTaskItem |
        ProjectPermissions.UpdateTaskItem |
        ProjectPermissions.DeleteTaskItem |
        ProjectPermissions.AssignTaskItem |
        ProjectPermissions.CreateTaskItemRelation |
        ProjectPermissions.UpdateTaskItemRelation |
        ProjectPermissions.DeleteTaskItemRelation |
        ProjectPermissions.CreateTaskItemBlockage |
        ProjectPermissions.UpdateTaskItemBlockage |
        ProjectPermissions.DeleteTaskItemBlockage;

    public static readonly ProjectPermissions ProjectContributorPermissions =
        ProjectPermissions.CreateHealthUpdate |
        ProjectPermissions.UpdateHealthUpdate |
        ProjectPermissions.CreateMilestone |
        ProjectPermissions.UpdateMilestone |
        ProjectPermissions.CreateWikiSectionPage |
        ProjectPermissions.UpdateWikiSectionPage |
        ProjectPermissions.CreateIteration |
        ProjectPermissions.UpdateIteration |
        ProjectPermissions.CreateTaskItem |
        ProjectPermissions.UpdateTaskItem |
        ProjectPermissions.AssignTaskItem;
}