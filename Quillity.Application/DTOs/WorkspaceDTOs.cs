using Quillity.Common.Values;

namespace Quillity.Application.DTOs;

public class WorkspaceDTO : DescriptiveDTOBase
{
    public required string UrlKey { get; set; }
    public string? LogoImageUrl { get; set; } = null;
    public required string InitialsForFallback { get; set; }
    public ICollection<WorkspaceMemberDTO> Members { get; set; } = [];
}

public class WorkspaceRoleDTO : DTOBase
{
    public required string Name { get; set; }
    public int HierarchyIndex { get; set; }
    public WorkspacePermissions Permissions { get; set; }
}

public class WorkspaceMemberDTO : MemberDTOBase
{
    public required WorkspaceRoleDTO Role { get; set; }
}

public class StatusDTO : ColorCodedItemDTOBase
{
    public StatusItemGroup Group { get; set; }
}

public class PriorityDTO : ColorCodedItemDTOBase
{
    public int MagnitudeIndex { get; set; }
}

public class LabelDTO : ColorCodedItemDTOBase
{
    public string? Description { get; set; }
}

public class EstimateDTO : DTOBase
{
    public required string Name { get; set; }
    public ICollection<EstimateItemDTO> Items { get; set; } = [];
}

public class EstimateItemDTO : DTOBase
{
    public required string Label { get; set; }
    public int MagnitudeIndex { get; set; }
}
