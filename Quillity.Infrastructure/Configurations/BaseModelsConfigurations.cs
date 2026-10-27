using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quillity.Common.Models;

namespace Quillity.Infrastructure.Configurations;

public class ModelBaseConfiguration<T> : IEntityTypeConfiguration<T> where T : ModelBase
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ConcurrencyStamp)
            .IsConcurrencyToken()
            .IsRequired();

        builder.Property(x => x.CreatedAtMs).IsRequired();
        builder.Property(x => x.CreatedBy).IsRequired();
        builder.Property(x => x.UpdatedAtMs).IsRequired();
        builder.Property(x => x.UpdatedBy).IsRequired();
    }
}

public class MemberModelBaseConfiguration<T> : ModelBaseConfiguration<T>, IEntityTypeConfiguration<T> where T : MemberModelBase
{
    public override void Configure(EntityTypeBuilder<T> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(x => x.JoinedAtMs).IsRequired();
        builder.Property(x => x.IsActive).IsRequired();
        builder.Property(x => x.LastActivityAtMs).IsRequired();
    }
}

public class DescriptiveModelBaseConfiguration<T> : ModelBaseConfiguration<T>, IEntityTypeConfiguration<T> where T : DescriptiveModelBase
{
    public override void Configure(EntityTypeBuilder<T> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.Description).IsRequired();
    }
}

public class PageModelBaseConfiguration<T> : ModelBaseConfiguration<T>, IEntityTypeConfiguration<T> where T : PageModelBase
{
    public override void Configure(EntityTypeBuilder<T> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Title).IsRequired();
        builder.Property(x => x.Content).IsRequired();
    }
}

public class ColorCodedItemModelBaseConfiguration<T> : ModelBaseConfiguration<T>, IEntityTypeConfiguration<T> where T : ColorCodedItemModelBase
{
    public override void Configure(EntityTypeBuilder<T> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.ColorCode).IsRequired();
    }
}

public class TimedItemModelBaseConfiguration<T> : DescriptiveModelBaseConfiguration<T>, IEntityTypeConfiguration<T> where T : TimedItemModelBase
{
    public override void Configure(EntityTypeBuilder<T> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.StartDateMs);
        builder.Property(x => x.TargetDateMs);
        builder.Property(x => x.EndDateMs);
    }
}

public class CommentModelBaseConfiguration<T> : ModelBaseConfiguration<T>, IEntityTypeConfiguration<T> where T : CommentModelBase
{
    public override void Configure(EntityTypeBuilder<T> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Content).IsRequired();
        builder.Property(x => x.SentAtMs).IsRequired();
        builder.Property(x => x.IsThreadResolved).IsRequired();
        builder.Property(x => x.ThreadResolvedAtMs).IsRequired();
    }
}

public class ReactionModelBaseConfiguration<T> : ModelBaseConfiguration<T>, IEntityTypeConfiguration<T> where T : ReactionModelBase
{
    public override void Configure(EntityTypeBuilder<T> builder)
    {
        base.Configure(builder);

        builder.Property(x => x.Emoji).IsRequired();
        builder.Property(x => x.ReactedAtMs).IsRequired();
    }
}