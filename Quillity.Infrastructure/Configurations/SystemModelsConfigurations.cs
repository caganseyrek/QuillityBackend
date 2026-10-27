using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quillity.Common.Models;
using Quillity.Common.Values;

namespace Quillity.Infrastructure.Configurations;

public class UserSessionModelConfiguration : ModelBaseConfiguration<UserSessionModel>, IEntityTypeConfiguration<UserSessionModel>
{
    public override void Configure(EntityTypeBuilder<UserSessionModel> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(UserSessionModel));

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(x => x.RefreshTokenHash).IsRequired();
        builder.Property(x => x.UserAgent);
        builder.Property(x => x.IPAddress);
        builder.Property(x => x.FirstLoggedInAtMs).IsRequired();
        builder.Property(x => x.LastActiveAtMs).IsRequired();
        builder.Property(x => x.IsRevoked).IsRequired();
        builder.Property(x => x.DeviceName);
    }
}

public class UserCreatedApiKeyModelConfiguration : ModelBaseConfiguration<UserCreatedApiKeyModel>, IEntityTypeConfiguration<UserCreatedApiKeyModel>
{
    public override void Configure(EntityTypeBuilder<UserCreatedApiKeyModel> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(UserCreatedApiKeyModel));

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(Constraints.NAME_STRING_LENGTH)
            .IsRequired();

        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.ExpiresAtMs);
        builder.Property(x => x.IsRevoked).IsRequired();
    }
}

public class ActivityLogModelConfiguration : IEntityTypeConfiguration<ActivityLogModel>
{
    public void Configure(EntityTypeBuilder<ActivityLogModel> builder)
    {
        builder.ToTable(nameof(ActivityLogModel));

        builder.HasKey(x => x.Id);
        builder.Property(x => x.ConcurrencyStamp)
            .IsConcurrencyToken()
            .IsRequired();

        builder.Property(x => x.ActorId).IsRequired();
        builder.Property(x => x.ActorName).IsRequired();
        builder.Property(x => x.ActivityDescription).IsRequired();

        builder.Property(x => x.ActivityType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.ActivityVisibility)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.WorkspaceId);
        builder.Property(x => x.ProjectId);
        builder.Property(x => x.InvokedAt).IsRequired();
        builder.Property(x => x.TimeStampMs).IsRequired();
    }
}