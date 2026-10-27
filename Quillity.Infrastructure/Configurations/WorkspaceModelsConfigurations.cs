using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quillity.Common.Models;
using Quillity.Common.Values;

namespace Quillity.Infrastructure.Configurations;

public class WorkspaceModelConfiguration : DescriptiveModelBaseConfiguration<WorkspaceModel>, IEntityTypeConfiguration<WorkspaceModel>
{
    public override void Configure(EntityTypeBuilder<WorkspaceModel> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(WorkspaceModel));

        builder.Property(x => x.UrlKey)
            .HasMaxLength(Constraints.URL_KEY_LENGTH)
            .IsRequired();
        
        builder.Property(x => x.LogoImageUrl).IsRequired();

        builder.HasOne(x => x.OwnerRole)
            .WithOne()
            .HasForeignKey<WorkspaceModel>(x => x.OwnerRoleId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(x => x.DefaultRole)
            .WithOne()
            .HasForeignKey<WorkspaceModel>(x => x.DefaultRoleId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(x => x.CustomRolesEnabled).IsRequired();
    }
}

public class WorkspaceRoleModelConfiguration : ModelBaseConfiguration<WorkspaceRoleModel>, IEntityTypeConfiguration<WorkspaceRoleModel>
{
    public override void Configure(EntityTypeBuilder<WorkspaceRoleModel> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.Workspace)
            .WithMany()
            .HasForeignKey(x => x.WorkspaceId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(Constraints.NAME_STRING_LENGTH)
            .IsRequired();

        builder.Property(x => x.HierarchyIndex).IsRequired();
        builder.Property(x => x.Permissions)
            .HasConversion<int>()
            .IsRequired();
    }
}

public class WorkspaceMemberModelConfiguration : MemberModelBaseConfiguration<WorkspaceMemberModel>, IEntityTypeConfiguration<WorkspaceMemberModel>
{
    public override void Configure(EntityTypeBuilder<WorkspaceMemberModel> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(WorkspaceMemberModel));

        builder.HasOne(x => x.Workspace)
            .WithMany(x => x.Members)
            .HasForeignKey(x => x.WorkspaceId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(x => x.Role)
            .WithMany()
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}

public class StatusModelConfiguration : ColorCodedItemModelBaseConfiguration<StatusModel>, IEntityTypeConfiguration<StatusModel>
{
    public override void Configure(EntityTypeBuilder<StatusModel> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(StatusModel));

        builder.HasOne(x => x.Workspace)
            .WithMany()
            .HasForeignKey(x => x.WorkspaceId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(x => x.Group)
            .HasConversion<int>()
            .IsRequired();
    }
}

public class PriorityModelConfiguration : ColorCodedItemModelBaseConfiguration<PriorityModel>, IEntityTypeConfiguration<PriorityModel>
{
    public override void Configure(EntityTypeBuilder<PriorityModel> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(PriorityModel));

        builder.HasOne(x => x.Workspace)
            .WithMany()
            .HasForeignKey(x => x.WorkspaceId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(x => x.MagnitudeIndex).IsRequired();
    }
}

public class LabelModelConfiguration : ColorCodedItemModelBaseConfiguration<LabelModel>, IEntityTypeConfiguration<LabelModel>
{
    public override void Configure(EntityTypeBuilder<LabelModel> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(LabelModel));

        builder.HasOne(x => x.Workspace)
            .WithMany()
            .HasForeignKey(x => x.WorkspaceId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(x => x.Description).IsRequired();
    }
}

public class EstimateModelConfiguration : ModelBaseConfiguration<EstimateModel>, IEntityTypeConfiguration<EstimateModel>
{
    public override void Configure(EntityTypeBuilder<EstimateModel> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(EstimateModel));

        builder.HasOne(x => x.Workspace)
            .WithMany()
            .HasForeignKey(x => x.WorkspaceId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(x => x.Name).IsRequired();
    }
}

public class EstimateItemModelConfiguration : ModelBaseConfiguration<EstimateItemModel>, IEntityTypeConfiguration<EstimateItemModel>
{
    public override void Configure(EntityTypeBuilder<EstimateItemModel> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(EstimateItemModel));

        builder.HasOne(x => x.Estimate)
            .WithMany(x => x.Items)
            .HasForeignKey(x => x.EstimateId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(x => x.Label).IsRequired();
        builder.Property(x => x.MagnitudeIndex).IsRequired();
    }
}
