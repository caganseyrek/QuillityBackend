using Quillity.Common.Values;

namespace Quillity.Application.DTOs;

public class ProjectDTO : DescriptiveDTOBase
{
    public required string UrlKey { get; set; }
    public required string TimeZone { get; set; }
    public required string IconName { get; set; }
    public StatusDTO? Status { get; set; }
    public ICollection<ProjectMemberDTO> Members { get; set; } = [];
    public bool IsArchived { get; set; } = false;
    public bool CustomRolesEnabled { get; set; } = false;
}

public class ProjectRoleDTO : DTOBase
{
    public required string Name { get; set; }
    public int HierarchyIndex { get; set; }
    public ProjectPermissions Permissions { get; set; }
}

public class ProjectMemberDTO : MemberDTOBase
{
    public required ProjectRoleDTO Role { get; set; }
}

public class MilestoneDTO : DescriptiveDTOBase
{
    public long DueDateMs { get; set; }
    public int ProgressPercentage { get; set; }
}

public class IterationDTO : TimedItemDTOBase
{
    public required ProjectDTO Project { get; set; }
    public string? EndGoal { get; set; } = null;
    public int PlannedItemCount { get; set; }
    public int CompletedItemCount { get; set; }
    public int VelocityPoints { get; set; }
    public string? Restrospective { get; set; } = null;
    public required StatusDTO Status { get; set; }
}

public class HealthUpdateDTO : DTOBase
{
    public required ProjectMemberDTO Author { get; set; }
    public HealthUpdateStatus Status { get; set; }
    public required string Summary { get; set; }
    public long PublishedAtMs { get; set; }
    public ICollection<HealthUpdateReactionDTO> Reactions { get; set; } = [];
    public ICollection<HealthUpdateCommentDTO> Comments { get; set; } = [];
}

public class HealthUpdateReactionDTO : ReactionDTOBase
{
    public required ProjectMemberDTO Author { get; set; }
}

public class HealthUpdateCommentDTO : CommentDTOBase
{
    public required ProjectMemberDTO Author { get; set; }
    public ICollection<HealthUpdateCommentDTO> Replies { get; set; } = [];
    public ICollection<HealthUpdateCommentReactionDTO> Reactions { get; set; } = [];
}

public class HealthUpdateCommentReactionDTO : ReactionDTOBase
{
    public required ProjectMemberDTO Author { get; set; }
}

public class WikiSectionDTO : DescriptiveDTOBase
{
    public ICollection<WikiSectionPageDTO> Pages { get; set; } = [];
    public bool IsDeprecated { get; set; }
    public bool IsLocked { get; set; }
}

public class WikiSectionPageDTO : PageDTOBase
{
    public bool IsDeprecated { get; set; }
}
