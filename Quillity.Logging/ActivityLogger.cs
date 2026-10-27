using Quillity.Common.Context;
using Quillity.Common.Models;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Logging;

public class ActivityLogger
{
    private readonly QuillityDbContext _dbContext;
    private readonly UserContext _userContext;

    public ActivityLogger(QuillityDbContext dbContext, UserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task LogActivityAsync(
        Type invoker,
        string activityDescription,
        ActivityLogType activityType,
        ActivityLogVisibility activityVisibility,
        Guid? workspaceId = null,
        Guid? projectId = null,
        CancellationToken cancellationToken = default)
    {
        var activityLog = new ActivityLogModel
        {
            ActorId = activityVisibility.Equals(ActivityLogVisibility.SystemInternal) ? Guid.Empty : _userContext.User.Id,
            ActorName = activityVisibility.Equals(ActivityLogVisibility.SystemInternal) ? "System" : _userContext.User.Email,
            ActivityDescription = activityDescription,
            ActivityType = activityType,
            ActivityVisibility = activityVisibility,
            WorkspaceId = workspaceId,
            ProjectId = projectId,
            InvokedAt = invoker.FullName ?? invoker.Name,
        };

        await _dbContext.ActivityLogs.AddAsync(activityLog, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
