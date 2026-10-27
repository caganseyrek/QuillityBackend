namespace Quillity.Common.Models;

public class ModelBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ConcurrencyStamp { get; set; } = Guid.NewGuid();
    public long CreatedAtMs { get; set; }
    public Guid CreatedBy { get; set; }
    public long UpdatedAtMs { get; set; }
    public Guid UpdatedBy { get; set; }
}

public class MemberModelBase : ModelBase
{
    public Guid UserId { get; set; }
    public required UserModel User { get; set; }
    public long JoinedAtMs { get; set; }
    public bool IsActive { get; set; }
    public long LastActivityAtMs { get; set; }
}

public class DescriptiveModelBase : ModelBase
{
    public required string Name { get; set; }
    public required string Description { get; set; } = string.Empty;
}

public class PageModelBase : ModelBase
{
    public required string Title { get; set; }
    public required string Content { get; set; } = string.Empty;
}

public class ColorCodedItemModelBase : ModelBase
{
    public required string Name { get; set; }
    public required string ColorCode { get; set; }
}

public class TimedItemModelBase : DescriptiveModelBase
{
    public long? StartDateMs { get; set; } = null;
    public long? TargetDateMs { get; set; } = null;
    public long? EndDateMs { get; set; } = null;
}

public class CommentModelBase : ModelBase
{
    public required string Content { get; set; }
    public long SentAtMs { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    public bool IsThreadResolved { get; set; } = false;
    public long? ThreadResolvedAtMs { get; set; } = null;
}

public class ReactionModelBase : ModelBase
{
    public required string Emoji { get; set; }
    public long ReactedAtMs { get; set; } = DateTimeOffset.Now.ToUnixTimeMilliseconds();
}
