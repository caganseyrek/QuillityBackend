using Quillity.Common.Values;

namespace Quillity.Common.Models;

public class ProjectModel : TimedItemModelBase
{
    public Guid WorkspaceId { get; set; }
    public required WorkspaceModel Workspace { get; set; }
    public required string UrlKey { get; set; }
    public required string TimeZone { get; set; }
    public required string IconName { get; set; }
    public Guid? StatusId { get; set; } = null;
    public StatusModel? Status { get; set; } = null;
    public ICollection<ProjectMemberModel> Members { get; set; } = [];
    public Guid LeadRoleId { get; set; }
    public ProjectRoleModel? LeadRole { get; set; } = null;
    public Guid DefaultRoleId { get; set; }
    public ProjectRoleModel? DefaultRole { get; set; } = null;
    public bool IsArchived { get; set; } = false;
    public bool CustomRolesEnabled { get; set; } = false;
}

public class ProjectRoleModel : ModelBase
{
    public Guid ProjectId { get; set; }
    public required ProjectModel Project { get; set; }
    public required string Name { get; set; }
    public int HierarchyIndex { get; set; } = int.MaxValue; // Lower value indicates higher authority
    public ProjectPermissions Permissions { get; set; }
}

public class ProjectMemberModel : MemberModelBase
{
    public Guid ProjectId { get; set; }
    public required ProjectModel Project { get; set; }
    public Guid RoleId { get; set; }
    public required ProjectRoleModel Role { get; set; }
}

public class MilestoneModel : DescriptiveModelBase
{
    public Guid ProjectId { get; set; }
    public required ProjectModel Project { get; set; }
    public long DueDateMs { get; set; }
    public int ProgressPercentage { get; set; } = 0;
}

public class IterationModel : TimedItemModelBase
{
    public Guid ProjectId { get; set; }
    public required ProjectModel Project { get; set; }
    public string? EndGoal { get; set; } = null;
    public int PlannedItemCount { get; set; } = 0;
    public int CompletedItemCount { get; set; } = 0;
    public int VelocityPoints { get; set; } = 0;
    public string? Restrospective { get; set; } = null;
    public Guid StatusId { get; set; }
    public required StatusModel Status { get; set; }
}

public class HealthUpdateModel : ModelBase
{
    public Guid ProjectId { get; set; }
    public required ProjectModel Project { get; set; }
    public HealthUpdateStatus Status { get; set; }
    public required string Summary { get; set; }
    public long PublishedAtMs { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    public Guid AuthorId { get; set; }
    public required ProjectMemberModel Author { get; set; }
    public ICollection<HealthUpdateReactionModel> Reactions { get; set; } = [];
    public ICollection<HealthUpdateCommentModel> Comments { get; set; } = [];
}

public class HealthUpdateReactionModel : ReactionModelBase
{
    public Guid HealthUpdateId { get; set; }
    public required HealthUpdateModel HealthUpdate { get; set; }
    public Guid AuthorId { get; set; }
    public required ProjectMemberModel Author { get; set; }
}

public class HealthUpdateCommentModel : CommentModelBase
{
    public Guid HealthUpdateId { get; set; }
    public required HealthUpdateModel HealthUpdate { get; set; }
    public Guid AuthorId { get; set; }
    public required ProjectMemberModel Author { get; set; }
    public Guid? ParentCommentId { get; set; } = null;
    public HealthUpdateCommentModel? ParentComment { get; set; } = null;
    public ICollection<HealthUpdateCommentReactionModel> Reactions { get; set; } = [];
    public ICollection<HealthUpdateCommentModel> Replies { get; set; } = [];
}

public class HealthUpdateCommentReactionModel : ReactionModelBase
{
    public Guid HealthUpdateCommentId { get; set; }
    public required HealthUpdateCommentModel HealthUpdateComment { get; set; }
    public Guid AuthorId { get; set; }
    public required ProjectMemberModel Author { get; set; }
}

public class WikiSectionModel : DescriptiveModelBase
{
    public Guid ProjectId { get; set; }
    public required ProjectModel Project { get; set; }
    public ICollection<WikiSectionPageModel> Pages { get; set; } = [];
    public bool IsDeprecated { get; set; } = false;
    public bool IsLocked { get; set; } = false;
}

public class WikiSectionPageModel : PageModelBase
{
    public Guid WikiSectionId { get; set; }
    public required WikiSectionModel WikiSection { get; set; }
    public bool IsDeprecated { get; set; } = false;
}
