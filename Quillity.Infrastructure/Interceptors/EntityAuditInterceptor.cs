using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Quillity.Common.Context;
using Quillity.Common.Models;

namespace Quillity.Infrastructure.Interceptors;

public class EntityAuditInterceptor : SaveChangesInterceptor
{
    private readonly UserContext _currentUser;

    public EntityAuditInterceptor(UserContext currentUser)
    {
        _currentUser = currentUser;
    }

    private void UpdateAuditProperties(DbContext context)
    {
        long nowTimestampMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        foreach (var entry in context.ChangeTracker.Entries<ModelBase>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtMs = nowTimestampMs;
                entry.Entity.CreatedBy = _currentUser.User.Id;
                entry.Entity.ConcurrencyStamp = Guid.NewGuid();
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAtMs = nowTimestampMs;
                entry.Entity.UpdatedBy = _currentUser.User.Id;
                entry.Entity.ConcurrencyStamp = Guid.NewGuid();
            }
        }
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
            UpdateAuditProperties(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
            UpdateAuditProperties(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
