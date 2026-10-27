using Quillity.Common.Models;

namespace Quillity.Application.DTOs;

public class DTOBase
{
    public Guid Id { get; set; }
}

public class MemberDTOBase : DTOBase
{
    public required UserDetailsDTO User;
    public long JoinedAtMs { get; set; }
    public bool IsActive { get; set; }
    public long LastActivityAtMs { get; set; }
}

public class DescriptiveDTOBase : DTOBase
{
    public required string Name { get; set; }
    public required string Description { get; set; }
}

public class PageDTOBase : DTOBase
{
    public required string Title { get; set; }
    public required string Content { get; set; }
}

public class ColorCodedItemDTOBase : DTOBase
{
    public required string Name { get; set; }
    public required string ColorCode { get; set; }
}

public class TimedItemDTOBase : DescriptiveDTOBase
{
    public long? StartDateMs { get; set; } = null;
    public long? TargetDateMs { get; set; } = null;
    public long? EndDateMs { get; set; } = null;
}

public class CommentDTOBase : ModelBase
{
    public required string Content { get; set; }
    public long SentAtMs { get; set; }
    public bool IsThreadResolved { get; set; }
    public long? ThreadResolvedAtMs { get; set; } = null;
}

public class ReactionDTOBase : ModelBase
{
    public required string Emoji { get; set; }
    public long ReactedAtMs { get; set; }
}
