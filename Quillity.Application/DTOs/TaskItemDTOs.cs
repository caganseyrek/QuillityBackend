namespace Quillity.Application.DTOs;

public class TaskItemDTO : TimedItemDTOBase
{
    public required ProjectDTO Project { get; set; }
    public MilestoneDTO? Milestone { get; set; }
    public IterationDTO? Iteration { get; set; }
    public PriorityDTO? Priority { get; set; }
    public EstimateItemDTO? Estimate { get; set; }
    public required StatusDTO Status { get; set; }
    public ProjectMemberDTO? ClosedBy { get; set; } = null;
    public ICollection<ProjectMemberDTO> Assignees { get; set; } = [];
    public ICollection<LabelDTO> Labels { get; set; } = [];
    public bool RequireAllSubsToBeComplete { get; set; }
    public ICollection<TaskItemDTO> SubTaskItems { get; set; } = [];
    public ICollection<TaskItemDTO> RelatedTaskItems { get; set; } = [];
    public ICollection<TaskItemDTO> RelatedToTaskItems { get; set; } = [];
    public ICollection<TaskItemDTO> BlockingItems { get; set; } = [];
    public ICollection<TaskItemDTO> BlockedByItems { get; set; } = [];
    public ICollection<TaskItemReactionDTO> Reactions { get; set; } = [];
    public ICollection<TaskItemCommentDTO> Comments { get; set; } = [];
}

public class TaskItemReactionDTO : ReactionDTOBase
{
    public required ProjectMemberDTO Author { get; set; }
}

public class TaskItemCommentDTO : CommentDTOBase
{
    public required ProjectMemberDTO Author { get; set; }
    public ICollection<TaskItemCommentDTO> Replies { get; set; } = [];
    public ICollection<TaskItemCommentReactionDTO> Reactions { get; set; } = [];
}

public class TaskItemCommentReactionDTO : ReactionDTOBase
{
    public required ProjectMemberDTO Author { get; set; }
}
