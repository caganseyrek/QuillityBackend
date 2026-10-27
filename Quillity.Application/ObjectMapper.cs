using Quillity.Application.DTOs;
using Quillity.Common.Models;

namespace Quillity.Application;

public static class ObjectMapper
{
    public static UserDetailsDTO ToUserDetailsDTO(UserModel userModel) => new()
    {
        Id = userModel.Id,
        UserName = userModel.UserName,
        FullName = userModel.FullName,
        Email = userModel.Email,
        ProfilePictureUrl = userModel.ProfilePictureUrl,
        InitialsForFallback = userModel.InitialsForFallback,
    };

    public static UserPreferencesDTO ToUserPreferencesDTO(UserPreferencesModel userPreferencesModel) => new()
    {
        PreferredUITheme = userPreferencesModel.PreferredUiTheme,
        UserTimeZone = userPreferencesModel.UserTimeZone,
        FirstDayOfWeek = userPreferencesModel.FirstDayOfWeek,
        DateFormat = userPreferencesModel.DateFormat,
        TimeFormat = userPreferencesModel.TimeFormat,
        Use24HourClock = userPreferencesModel.Use24HourClock,
        AutoAssignNewTasksToSelf = userPreferencesModel.AutoAssignNewTasksToSelf,
    };

    public static InvitationDTO ToInvitationDTO(InvitationModel invitationModel) => new()
    {
        Id = invitationModel.Id,
        InvitedUser = ToUserDetailsDTO(invitationModel.InvitedUser),
        InvitedWorkspace = ToWorkspaceDTO(invitationModel.InvitedWorkspace),
        InvitedByUser = ToUserDetailsDTO(invitationModel.InvitedByUser),
        InvitedAtMs = invitationModel.InvitedAtMs,
        InvitationNote = invitationModel.InvitationNote,
        AcceptedAtMs = invitationModel.AcceptedAtMs,
    };

    public static NotepadPageDTO ToNotepadPageDTO(NotepadPageModel notepadPageModel) => new()
    {
        Id = notepadPageModel.Id,
        Title = notepadPageModel.Title,
        Content = notepadPageModel.Content,
    };

    public static WorkspaceDTO ToWorkspaceDTO(WorkspaceModel workspaceModel) => new()
    {
        Id = workspaceModel.Id,
        Name = workspaceModel.Name,
        Description = workspaceModel.Description,
        UrlKey = workspaceModel.UrlKey,
        LogoImageUrl = workspaceModel.LogoImageUrl,
        InitialsForFallback = workspaceModel.InitialsForFallback,
        Members = workspaceModel.Members.Select(ToWorkspaceMemberDTO).ToList(),
    };

    public static WorkspaceRoleDTO ToWorkspaceRoleDTO(WorkspaceRoleModel workspaceRoleModel) => new()
    {
        Id = workspaceRoleModel.Id,
        Name = workspaceRoleModel.Name,
        Permissions = workspaceRoleModel.Permissions,
        HierarchyIndex = workspaceRoleModel.HierarchyIndex,
    };

    public static WorkspaceMemberDTO ToWorkspaceMemberDTO(WorkspaceMemberModel workspaceMemberModel) => new()
    {
        Id = workspaceMemberModel.Id,
        User = ToUserDetailsDTO(workspaceMemberModel.User),
        Role = ToWorkspaceRoleDTO(workspaceMemberModel.Role),
    };

    public static EstimateDTO ToEstimateDTO(EstimateModel estimateModel) => new()
    {
        Id = estimateModel.Id,
        Name = estimateModel.Name,
        Items = estimateModel.Items.Select(ToEstimateItemDTO).ToList(),
    };

    public static EstimateItemDTO ToEstimateItemDTO(EstimateItemModel estimateItemModel) => new()
    {
        Id = estimateItemModel.Id,
        Label = estimateItemModel.Label,
        MagnitudeIndex = estimateItemModel.MagnitudeIndex,
    };

    public static LabelDTO ToLabelDTO(LabelModel labelModel) => new()
    {
        Id = labelModel.Id,
        Name = labelModel.Name,
        Description = labelModel.Description,
        ColorCode = labelModel.ColorCode,
    };

    public static PriorityDTO ToPriorityDTO(PriorityModel priorityModel) => new()
    {
        Id = priorityModel.Id,
        Name = priorityModel.Name,
        ColorCode = priorityModel.ColorCode,
        MagnitudeIndex = priorityModel.MagnitudeIndex,
    };

    public static StatusDTO ToStatusDTO(StatusModel statusModel) => new()
    {
        Id = statusModel.Id,
        Name = statusModel.Name,
        ColorCode = statusModel.ColorCode,
        Group = statusModel.Group,
    };

    public static ProjectDTO ToProjectDTO(ProjectModel projectModel) => new()
    {
        Id = projectModel.Id,
        Name = projectModel.Name,
        Description = projectModel.Description,
        TimeZone = projectModel.TimeZone,
        UrlKey = projectModel.UrlKey,
        IconName = projectModel.IconName,
        CustomRolesEnabled = projectModel.CustomRolesEnabled,
        Status = projectModel.Status != null ? ToStatusDTO(projectModel.Status) : null,
        IsArchived = projectModel.IsArchived,
        Members = projectModel.Members.Select(ToProjectMemberDTO).ToList(),
    };

    public static ProjectRoleDTO ToProjectRoleDTO(ProjectRoleModel projectRoleModel) => new()
    {
        Id = projectRoleModel.Id,
        Name = projectRoleModel.Name,
        Permissions = projectRoleModel.Permissions,
        HierarchyIndex = projectRoleModel.HierarchyIndex,
    };

    public static ProjectMemberDTO ToProjectMemberDTO(ProjectMemberModel projectMemberModel) => new()
    {
        Id = projectMemberModel.Id,
        User = ToUserDetailsDTO(projectMemberModel.User),
        Role = ToProjectRoleDTO(projectMemberModel.Role),
    };

    public static HealthUpdateDTO ToHealthUpdateDTO(HealthUpdateModel healthUpdateModel) => new()
    {
        Id = healthUpdateModel.Id,
        Author = ToProjectMemberDTO(healthUpdateModel.Author),
        Summary = healthUpdateModel.Summary,
        Status = healthUpdateModel.Status,
        PublishedAtMs = healthUpdateModel.PublishedAtMs,
        Reactions = healthUpdateModel.Reactions
            .OrderBy(reaction => reaction.ReactedAtMs)
            .Select(ToHealthUpdateReactionDTO)
            .ToList(),
        Comments = CommentTreeBuilder
            .ProcessComments(healthUpdateModel.Comments.ToList())
            .OrderBy(comment => comment.SentAtMs)
            .Select(ToHealthUpdateCommentDTO)
            .ToList(),
    };

    public static HealthUpdateReactionDTO ToHealthUpdateReactionDTO(HealthUpdateReactionModel healthUpdateReactionModel) => new()
    {
        Id = healthUpdateReactionModel.Id,
        Author = ToProjectMemberDTO(healthUpdateReactionModel.Author),
        Emoji = healthUpdateReactionModel.Emoji,
        ReactedAtMs = healthUpdateReactionModel.ReactedAtMs,
    };

    public static HealthUpdateCommentDTO ToHealthUpdateCommentDTO(HealthUpdateCommentModel healthUpdateCommentModel) => new()
    {
        Id = healthUpdateCommentModel.Id,
        Author = ToProjectMemberDTO(healthUpdateCommentModel.Author),
        Content = healthUpdateCommentModel.Content,
        SentAtMs = healthUpdateCommentModel.SentAtMs,
        IsThreadResolved = healthUpdateCommentModel.IsThreadResolved,
        ThreadResolvedAtMs = healthUpdateCommentModel.ThreadResolvedAtMs,
        Reactions = healthUpdateCommentModel.Reactions
            .OrderBy(reaction => reaction.ReactedAtMs)
            .Select(ToHealthUpdateCommentReactionDTO)
            .ToList(),
        Replies = CommentTreeBuilder
            .ProcessComments(healthUpdateCommentModel.Replies.ToList())
            .OrderBy(comment => comment.SentAtMs)
            .Select(ToHealthUpdateCommentDTO)
            .ToList(),
    };

    public static HealthUpdateCommentReactionDTO ToHealthUpdateCommentReactionDTO(HealthUpdateCommentReactionModel healthUpdateCommentReactionModel) => new()
    {
        Id = healthUpdateCommentReactionModel.Id,
        Author = ToProjectMemberDTO(healthUpdateCommentReactionModel.Author),
        Emoji = healthUpdateCommentReactionModel.Emoji,
        ReactedAtMs = healthUpdateCommentReactionModel.ReactedAtMs,
    };

    public static WikiSectionDTO ToWikiSectionDTO(WikiSectionModel wikiSectionModel) => new()
    {
        Id = wikiSectionModel.Id,
        Name = wikiSectionModel.Name,
        Description = wikiSectionModel.Description,
        Pages = wikiSectionModel.Pages
            .Select(ToWikiSectionPageDTO)
            .ToList(),
        IsDeprecated = wikiSectionModel.IsDeprecated,
        IsLocked = wikiSectionModel.IsLocked,
    };

    public static WikiSectionPageDTO ToWikiSectionPageDTO(WikiSectionPageModel wikiSectionPageModel) => new()
    {
        Id = wikiSectionPageModel.Id,
        Title = wikiSectionPageModel.Title,
        Content = wikiSectionPageModel.Content,
        IsDeprecated = wikiSectionPageModel.IsDeprecated,
    };

    public static IterationDTO ToIterationDTO(IterationModel iterationModel) => new()
    {
        Id = iterationModel.Id,
        Name = iterationModel.Name,
        Description = iterationModel.Description,
        Project = ToProjectDTO(iterationModel.Project),
        StartDateMs = iterationModel.StartDateMs,
        TargetDateMs = iterationModel.TargetDateMs,
        EndDateMs = iterationModel.EndDateMs,
        EndGoal = iterationModel.EndGoal,
        PlannedItemCount = iterationModel.PlannedItemCount,
        CompletedItemCount = iterationModel.CompletedItemCount,
        VelocityPoints = iterationModel.VelocityPoints,
        Restrospective = iterationModel.Restrospective,
        Status = ToStatusDTO(iterationModel.Status),
    };

    public static MilestoneDTO ToMilestoneDTO(MilestoneModel milestoneModel) => new()
    {
        Id = milestoneModel.Id,
        Name = milestoneModel.Name,
        Description = milestoneModel.Description,
        DueDateMs = milestoneModel.DueDateMs,
        ProgressPercentage = milestoneModel.ProgressPercentage,
    };

    public static TaskItemDTO ToTaskItemDTO(TaskItemModel taskItemModel) => new()
    {
        Id = taskItemModel.Id,
        Name = taskItemModel.Name,
        Description = taskItemModel.Description,
        StartDateMs = taskItemModel.StartDateMs,
        TargetDateMs = taskItemModel.TargetDateMs,
        EndDateMs = taskItemModel.EndDateMs,
        Project = ToProjectDTO(taskItemModel.Project),
        Milestone = taskItemModel.Milestone == null ? null: ToMilestoneDTO(taskItemModel.Milestone),
        Iteration = taskItemModel.Iteration == null ? null: ToIterationDTO(taskItemModel.Iteration),
        Priority = taskItemModel.Priority == null ? null: ToPriorityDTO(taskItemModel.Priority),
        Estimate = taskItemModel.Estimate == null ? null: ToEstimateItemDTO(taskItemModel.Estimate),
        Status = ToStatusDTO(taskItemModel.Status),
        ClosedBy = taskItemModel.ClosedBy == null ? null : ToProjectMemberDTO(taskItemModel.ClosedBy),
        Assignees = taskItemModel.Assignees.Select(x => x.Member).Select(ToProjectMemberDTO).ToList(),
        Labels = taskItemModel.Labels.Select(x => x.Label).Select(ToLabelDTO).ToList(),
        RequireAllSubsToBeComplete = taskItemModel.RequireAllSubsToBeComplete,
        SubTaskItems = taskItemModel.SubTaskItems.Select(ToTaskItemDTO).ToList(),
        RelatedTaskItems = taskItemModel.RelatedTaskItems.Select(x => x.RelatedTaskItem).Select(ToTaskItemDTO).ToList(),
        RelatedToTaskItems = taskItemModel.RelatedToTaskItems.Select(x => x.RelatedToTaskItem).Select(ToTaskItemDTO).ToList(),
        BlockingItems = taskItemModel.BlockingItems.Select(x => x.BlockingTaskItem).Select(ToTaskItemDTO).ToList(),
        BlockedByItems = taskItemModel.BlockedByItems.Select(x => x.BlockingTaskItem).Select(ToTaskItemDTO).ToList(),
        Reactions = taskItemModel.Reactions.Select(ToTaskItemReactionDTO).ToList(),
        Comments = CommentTreeBuilder
            .ProcessComments(taskItemModel.Comments.ToList())
            .OrderBy(comment => comment.SentAtMs)
            .Select(ToTaskItemCommentDTO)
            .ToList()
    };

    public static TaskItemReactionDTO ToTaskItemReactionDTO(TaskItemReactionModel taskItemReactionModel) => new()
    {
        Id = taskItemReactionModel.Id,
        Author = ToProjectMemberDTO(taskItemReactionModel.Author),
        Emoji = taskItemReactionModel.Emoji,
        ReactedAtMs = taskItemReactionModel.ReactedAtMs,
    };

    public static TaskItemCommentDTO ToTaskItemCommentDTO(TaskItemCommentModel taskItemCommentModel) => new()
    {
        Id = taskItemCommentModel.Id,
        Author = ToProjectMemberDTO(taskItemCommentModel.Author),
        Content = taskItemCommentModel.Content,
        SentAtMs = taskItemCommentModel.SentAtMs,
        IsThreadResolved = taskItemCommentModel.IsThreadResolved,
        ThreadResolvedAtMs = taskItemCommentModel.ThreadResolvedAtMs,
        Reactions = taskItemCommentModel.Reactions
            .OrderBy(reaction => reaction.ReactedAtMs)
            .Select(ToTaskItemCommentReactionDTO)
            .ToList(),
        Replies = CommentTreeBuilder
            .ProcessComments(taskItemCommentModel.Replies.ToList())
            .OrderBy(comment => comment.SentAtMs)
            .Select(ToTaskItemCommentDTO)
            .ToList(),
    };

    public static TaskItemCommentReactionDTO ToTaskItemCommentReactionDTO(TaskItemCommentReactionModel taskItemCommentReactionModel) => new()
    {
        Id = taskItemCommentReactionModel.Id,
        Author = ToProjectMemberDTO(taskItemCommentReactionModel.Author),
        Emoji = taskItemCommentReactionModel.Emoji,
        ReactedAtMs = taskItemCommentReactionModel.ReactedAtMs,
    };

}
