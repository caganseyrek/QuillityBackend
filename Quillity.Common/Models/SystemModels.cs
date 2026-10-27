using Quillity.Common.Values;

namespace Quillity.Common.Models;

public class UserSessionModel : ModelBase
{
    public Guid UserId { get; set; }
    public required UserModel User { get; set; }
    public required string RefreshTokenHash { get; set; }
    public string? UserAgent { get; set; } = null;
    public string? IPAddress { get; set; } = null;
    public long FirstLoggedInAtMs { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    public long LastActiveAtMs { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    public string? DeviceName { get; set; } = null;
    public bool IsRevoked { get; set; } = false;
}

public class UserCreatedApiKeyModel : ModelBase
{
    public Guid UserId { get; set; }
    public required UserModel User { get; set; }
    public required string Name { get; set; }
    public required string KeyHash { get; set; }
    public long? ExpiresAtMs { get; set; } = null;
    public bool IsRevoked { get; set; } = false;
}

public class ActivityLogModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ConcurrencyStamp { get; set; } = Guid.NewGuid();
    public Guid ActorId { get; set; }
    public required string ActorName { get; set; }
    public required string ActivityDescription { get; set; }
    public ActivityLogType ActivityType { get; set; }
    public ActivityLogVisibility ActivityVisibility { get; set; }
    public Guid? WorkspaceId { get; set; } = null;
    public Guid? ProjectId { get; set; } = null;
    public required string InvokedAt { get; set; }
    public long TimeStampMs { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
}
