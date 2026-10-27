using Quillity.Common.Values;

namespace Quillity.Common.Models;

// Since role models require a workspace, we need to define the workspace model first,
// that is why owner and default roles are defined as nullable in the workspace model.
// But we set them after the workspace is created in the command handler, so they are never null in practice.
//
// In the role update command handler, we must ensure the owner and default roles are never null.
public class WorkspaceModel : DescriptiveModelBase
{
    public required string UrlKey { get; set; }
    public string? LogoImageUrl { get; set; } = null;
    public required string InitialsForFallback { get; set; }
    public ICollection<WorkspaceMemberModel> Members { get; set; } = [];
    public Guid OwnerRoleId { get; set; }
    public WorkspaceRoleModel? OwnerRole { get; set; }
    public Guid DefaultRoleId { get; set; }
    public WorkspaceRoleModel? DefaultRole { get; set; }
    public bool CustomRolesEnabled { get; set; } = false;
}

public class WorkspaceRoleModel : ModelBase
{
    public Guid WorkspaceId { get; set; }
    public required WorkspaceModel Workspace { get; set; }
    public required string Name { get; set; }
    public int HierarchyIndex { get; set; } = int.MaxValue;
    public WorkspacePermissions Permissions { get; set; }
}

public class WorkspaceMemberModel : MemberModelBase
{
    public Guid WorkspaceId { get; set; }
    public required WorkspaceModel Workspace { get; set; }
    public Guid RoleId { get; set; }
    public required WorkspaceRoleModel Role { get; set; }
}

public class StatusModel : ColorCodedItemModelBase
{
    public Guid WorkspaceId { get; set; }
    public required WorkspaceModel Workspace { get; set; }
    public StatusItemGroup Group { get; set; }
}

public class PriorityModel : ColorCodedItemModelBase
{
    public Guid WorkspaceId { get; set; }
    public required WorkspaceModel Workspace { get; set; }
    public int MagnitudeIndex { get; set; } = 0; // Lower values indicate higher priority
}

public class LabelModel : ColorCodedItemModelBase
{
    public Guid WorkspaceId { get; set; }
    public required WorkspaceModel Workspace { get; set; }
    public string? Description { get; set; } = null;
}

public class EstimateModel : ModelBase
{
    public Guid WorkspaceId { get; set; }
    public required WorkspaceModel Workspace { get; set; }
    public required string Name { get; set; }
    public ICollection<EstimateItemModel> Items { get; set; } = [];
}

public class EstimateItemModel : ModelBase
{
    public Guid EstimateId { get; set; }
    public required EstimateModel Estimate { get; set; }
    public required string Label { get; set; }
    public int MagnitudeIndex { get; set; } = 0; // Lower values indicate higher estimate
}
