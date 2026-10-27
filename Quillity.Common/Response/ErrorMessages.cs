using Quillity.Common.Helpers;
using Quillity.Common.Values;
using static System.Formats.Asn1.AsnWriter;

namespace Quillity.Common.Response;

public static class ErrorMessages
{
    public static string GetServerError() => "A server error occurred. Please try again later.";

    public static string GetForbiddenError(ErrorMessageScope scope = ErrorMessageScope.None)
    {
        string messageSuffix = scope switch
        {
            ErrorMessageScope.Workspace => " in this workspace",
            ErrorMessageScope.Project => " in this project",
            _ => "",
        };
        return $"You do not have permission to do this action{messageSuffix}.";
    }

    public static string GetEntryWithSamePropExistsError(
        ErrorMessageModel model, string propertyName, ErrorMessageScope scope = ErrorMessageScope.None)
    {
        string messageSuffix = scope switch
        {
            ErrorMessageScope.Workspace => " in this workspace",
            ErrorMessageScope.Project => " in this project",
            _ => "",
        };
        return $"{StringHelper.Capitalize(GetModelName(model))} with same {propertyName.ToLower()} already exists{messageSuffix}.";
    }

    public static string GetInvalidColorCodeError() => "Invalid color code format. Color code must be a valid hex color.";

    public static string GetNotFoundError(ErrorMessageModel model) =>
        $"{StringHelper.Capitalize(GetModelName(model))} not found.";

    public static string GetCannotBeEmptyError(ErrorMessageModel model, string propertyName) =>
        $"{StringHelper.Capitalize(GetModelName(model))} {propertyName.ToLower()} cannot be empty.";

    public static string GetStringLengthError(ErrorMessageModel model, string propertyName, int maxLength) =>
        $"{StringHelper.Capitalize(GetModelName(model))} {propertyName.ToLower()}" +
        $" should be between {Constraints.MIN_STRING_LENGTH} and {maxLength} characters long.";

    public static string GetUrlKeyLengthError(ErrorMessageModel model) =>
        $"{StringHelper.Capitalize(GetModelName(model))} URL key should be 8 characters long.";

    public static string GetUrlKeyCharacterError(ErrorMessageModel model) =>
        $"{StringHelper.Capitalize(GetModelName(model))} URL key must contain only letters.";

    public static string GetInvalidUrlError(ErrorMessageModel model, string propertyName) =>
        $"{StringHelper.Capitalize(GetModelName(model))} {propertyName.ToLower()} must be a valid URL.";

    public static string GetInvalidIdFormatError(ErrorMessageModel model) => $"{StringHelper.Capitalize(GetModelName(model))} id is invalid";

    public static string GetWorkspaceAccessLogicError() =>
        $"User was not found in workspace after 'AuthorizeWorkspaceAccessAsync' check. This indicates a logic error.";

    public static string GetProjectAccessLogicError() =>
        $"User was not found in workspace after 'AuthorizeWorkspaceAccessAsync' check. This indicates a logic error.";

    private static string GetModelName(ErrorMessageModel model) => model switch
    {
        ErrorMessageModel.User => "User",
        ErrorMessageModel.NotepadPage => "Notepad Page",
        ErrorMessageModel.Invitation => "Invitation",
        ErrorMessageModel.Workspace => "Workspace",
        ErrorMessageModel.WorkspaceRole => "Workspace Role",
        ErrorMessageModel.WorkspaceMember => "Workspace Member",
        ErrorMessageModel.Status => "Status",
        ErrorMessageModel.Priority => "Priority",
        ErrorMessageModel.Label => "Label",
        ErrorMessageModel.Estimate => "Estimate",
        ErrorMessageModel.EstimateItem => "Estimate Item",
        ErrorMessageModel.Project => "Project",
        ErrorMessageModel.ProjectRole => "Project Role",
        ErrorMessageModel.ProjectMember => "Project Member",
        ErrorMessageModel.HealthUpdate => "Project Health Update",
        ErrorMessageModel.HealthUpdateReaction => "Project Health Update Reaction",
        ErrorMessageModel.HealthUpdateComment => "Project Health Update Comment",
        ErrorMessageModel.Milestone => "Milestone",
        ErrorMessageModel.WikiSection => "Wiki Section",
        ErrorMessageModel.WikiSectionPage => "Wiki Section Page",
        ErrorMessageModel.Iteration => "Iteration",
        ErrorMessageModel.TaskItem => "Task Item",
        ErrorMessageModel.TaskItemRelation => "Task Item Relation",
        ErrorMessageModel.TaskItemBlockage => "Task Item Blockage",
        ErrorMessageModel.TaskItemComment => "Task Item Comment",
        ErrorMessageModel.TaskItemReaction => "Task Item Reaction",
        _ => "item",
    };
}

public enum ErrorMessageModel
{
    User = 0,
    NotepadPage = 1,
    Invitation = 2,
    Workspace = 3,
    WorkspaceRole = 4,
    WorkspaceMember = 5,
    Status = 6,
    Priority = 7,
    Label = 8,
    Estimate = 9,
    EstimateItem = 10,
    Project = 11,
    ProjectRole = 12,
    ProjectMember = 13,
    HealthUpdate = 14,
    HealthUpdateReaction = 15,
    HealthUpdateComment = 16,
    Milestone = 17,
    WikiSection = 18,
    WikiSectionPage = 19,
    Iteration = 20,
    TaskItem = 21,
    TaskItemRelation = 22,
    TaskItemBlockage = 23,
    TaskItemReaction = 24,
    TaskItemComment = 25,
}

public enum ErrorMessageScope
{
    None = 0,
    Workspace = 1,
    Project = 2,
}
