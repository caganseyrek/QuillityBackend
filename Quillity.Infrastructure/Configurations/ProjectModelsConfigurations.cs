using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Quillity.Common.Models;
using Quillity.Common.Values;

namespace Quillity.Infrastructure.Configurations;

public class ProjectModelConfiguration : DescriptiveModelBaseConfiguration<ProjectModel>, IEntityTypeConfiguration<ProjectModel>
{
    public override void Configure(EntityTypeBuilder<ProjectModel> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(ProjectModel));

        builder.Property(x => x.UrlKey).IsRequired();
        builder.Property(x => x.TimeZone).IsRequired();
        builder.Property(x => x.IconName).IsRequired();

        builder.HasOne(x => x.Status)
            .WithMany()
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(x => x.LeadRole)
            .WithOne()
            .HasForeignKey<ProjectModel>(x => x.LeadRoleId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(x => x.DefaultRole)
            .WithOne()
            .HasForeignKey<ProjectModel>(x => x.DefaultRoleId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(x => x.IsArchived).IsRequired();
        builder.Property(x => x.CustomRolesEnabled).IsRequired();
    }
}

public class ProjectRoleModelConfiguration : ModelBaseConfiguration<ProjectRoleModel>, IEntityTypeConfiguration<ProjectRoleModel>
{
    public override void Configure(EntityTypeBuilder<ProjectRoleModel> builder)
    {
        base.Configure(builder);

        builder.HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
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

public class ProjectMemberModelConfiguration : MemberModelBaseConfiguration<ProjectMemberModel>, IEntityTypeConfiguration<ProjectMemberModel>
{
    public override void Configure(EntityTypeBuilder<ProjectMemberModel> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(ProjectMemberModel));

        builder.HasOne(x => x.Project)
            .WithMany(x => x.Members)
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(x => x.Role)
            .WithMany()
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}

public class ProjectMilestoneModelConfiguration : DescriptiveModelBaseConfiguration<MilestoneModel>, IEntityTypeConfiguration<MilestoneModel>
{
    public override void Configure(EntityTypeBuilder<MilestoneModel> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(MilestoneModel));

        builder.HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(x => x.DueDateMs).IsRequired();
        builder.Property(x => x.ProgressPercentage).IsRequired();
    }
}

public class ProjectIterationModelConfiguration : TimedItemModelBaseConfiguration<IterationModel>, IEntityTypeConfiguration<IterationModel>
{
    public override void Configure(EntityTypeBuilder<IterationModel> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(IterationModel));

        builder.HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(x => x.EndGoal);
        builder.Property(x => x.PlannedItemCount).IsRequired();
        builder.Property(x => x.CompletedItemCount).IsRequired();
        builder.Property(x => x.VelocityPoints).IsRequired();
        builder.Property(x => x.Restrospective);

        builder.HasOne(x => x.Status)
            .WithMany()
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}

public class ProjectHealthUpdateModelConfiguration : ModelBaseConfiguration<HealthUpdateModel>, IEntityTypeConfiguration<HealthUpdateModel>
{
    public override void Configure(EntityTypeBuilder<HealthUpdateModel> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(HealthUpdateModel));

        builder.HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Summary).IsRequired();

        builder.HasOne(x => x.Author)
            .WithMany()
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(x => x.PublishedAtMs).IsRequired();
    }
}

public class ProjectHealthUpdateReactionModelConfiguration : ReactionModelBaseConfiguration<HealthUpdateReactionModel>, IEntityTypeConfiguration<HealthUpdateReactionModel>
{
    public override void Configure(EntityTypeBuilder<HealthUpdateReactionModel> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(HealthUpdateReactionModel));

        builder.HasOne(x => x.HealthUpdate)
            .WithMany(x => x.Reactions)
            .HasForeignKey(x => x.HealthUpdateId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(x => x.Author)
            .WithMany()
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}

public class ProjectHealthUpdateCommentModelConfiguration : CommentModelBaseConfiguration<HealthUpdateCommentModel>, IEntityTypeConfiguration<HealthUpdateCommentModel>
{
    public override void Configure(EntityTypeBuilder<HealthUpdateCommentModel> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(HealthUpdateCommentModel));

        builder.HasOne(x => x.HealthUpdate)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.HealthUpdateId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(x => x.Author)
            .WithMany()
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(x => x.ParentComment)
            .WithMany(x => x.Replies)
            .HasForeignKey(x => x.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ProjectHealthUpdateCommentReactionModelConfiguration : ReactionModelBaseConfiguration<HealthUpdateCommentReactionModel>, IEntityTypeConfiguration<HealthUpdateCommentReactionModel>
{
    public override void Configure(EntityTypeBuilder<HealthUpdateCommentReactionModel> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(HealthUpdateCommentReactionModel));

        builder.HasOne(x => x.HealthUpdateComment)
            .WithMany(x => x.Reactions)
            .HasForeignKey(x => x.HealthUpdateCommentId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(x => x.Author)
            .WithMany()
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}


public class ProjectWikiModelConfiguration : DescriptiveModelBaseConfiguration<WikiSectionModel>, IEntityTypeConfiguration<WikiSectionModel>
{
    public override void Configure(EntityTypeBuilder<WikiSectionModel> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(WikiSectionModel));

        builder.HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(x => x.IsDeprecated).IsRequired();
        builder.Property(x => x.IsLocked).IsRequired();
    }
}

public class ProjectWikiPageModelConfiguration : PageModelBaseConfiguration<WikiSectionPageModel>, IEntityTypeConfiguration<WikiSectionPageModel>
{
    public override void Configure(EntityTypeBuilder<WikiSectionPageModel> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(WikiSectionPageModel));

        builder.HasOne(x => x.WikiSection)
            .WithMany(x => x.Pages)
            .HasForeignKey(x => x.WikiSectionId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(x => x.IsDeprecated).IsRequired();
    }
}
