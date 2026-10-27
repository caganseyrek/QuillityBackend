namespace Quillity.Common.Models;

public class TaskItemModel : TimedItemModelBase
{
    public Guid ProjectId { get; set; }
    public required ProjectModel Project { get; set; }
    public Guid? MilestoneId { get; set; } = null;
    public MilestoneModel? Milestone { get; set; } = null;
    public Guid? IterationId { get; set; } = null;
    public IterationModel? Iteration { get; set; } = null;
    public Guid? PriorityId { get; set; } = null;
    public PriorityModel? Priority { get; set; } = null;
    public Guid? EstimateId { get; set; } = null;
    public EstimateItemModel? Estimate { get; set; } = null;
    public Guid StatusId { get; set; }
    public required StatusModel Status { get; set; }
    public Guid? ClosedById { get; set; } = null;
    public ProjectMemberModel? ClosedBy { get; set; } = null;
    public ICollection<TaskItemAssigneeModel> Assignees { get; set; } = [];
    public ICollection<TaskItemLabelModel> Labels { get; set; } = [];
    public Guid? ParentTaskItemId { get; set; } = null;
    public TaskItemModel? ParentTaskItem { get; set; } = null;
    public bool RequireAllSubsToBeComplete { get; set; } = false;
    public ICollection<TaskItemModel> SubTaskItems { get; set; } = [];
    public ICollection<TaskItemBlockageModel> BlockingItems { get; set; } = [];
    public ICollection<TaskItemBlockageModel> BlockedByItems { get; set; } = [];
    public ICollection<TaskItemRelationModel> RelatedTaskItems { get; set; } = [];
    public ICollection<TaskItemRelationModel> RelatedToTaskItems { get; set; } = [];
    public ICollection<TaskItemReactionModel> Reactions { get; set; } = [];
    public ICollection<TaskItemCommentModel> Comments { get; set; } = [];
}

public class TaskItemAssigneeModel : ModelBase
{
    public Guid TaskItemId { get; set; }
    public required TaskItemModel TaskItem { get; set; }
    public Guid MemberId { get; set; }
    public required ProjectMemberModel Member { get; set; }
}

public class TaskItemBlockageModel : ModelBase
{
    public Guid BlockingTaskItemId { get; set; }
    public required TaskItemModel BlockingTaskItem { get; set; }
    public Guid BlockedTaskItemId { get; set; }
    public required TaskItemModel BlockedTaskItem { get; set; }
}

public class TaskItemRelationModel : ModelBase
{
    public Guid RelatedTaskItemId { get; set; }
    public required TaskItemModel RelatedTaskItem { get; set; }
    public Guid RelatedToTaskItemId { get; set; }
    public required TaskItemModel RelatedToTaskItem { get; set; }
}


public class TaskItemLabelModel : ModelBase
{
    public Guid TaskItemId { get; set; }
    public required TaskItemModel TaskItem { get; set; }
    public Guid LabelId { get; set; }
    public required LabelModel Label { get; set; }
}

public class TaskItemReactionModel : ReactionModelBase
{
    public Guid TaskItemId { get; set; }
    public required TaskItemModel TaskItem { get; set; }
    public Guid AuthorId { get; set; }
    public required ProjectMemberModel Author { get; set; }
}

public class TaskItemCommentModel : CommentModelBase
{
    public Guid TaskItemId { get; set; }
    public required TaskItemModel TaskItem { get; set; }
    public Guid AuthorId { get; set; }
    public required ProjectMemberModel Author { get; set; }
    public Guid? ParentCommentId { get; set; } = null;
    public TaskItemCommentModel? ParentComment { get; set; } = null;
    public ICollection<TaskItemCommentModel> Replies { get; set; } = [];
    public ICollection<TaskItemCommentReactionModel> Reactions { get; set; } = [];
}

public class TaskItemCommentReactionModel : ReactionModelBase
{
    public Guid TaskItemCommentId { get; set; }
    public required TaskItemCommentModel TaskItemComment { get; set; }
    public Guid AuthorId { get; set; }
    public required ProjectMemberModel Author { get; set; }
}
