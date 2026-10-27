using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Quillity.Common.Models;

namespace Quillity.Infrastructure.Configurations;

public class TaskItemModelConfiguration : TimedItemModelBaseConfiguration<TaskItemModel>, IEntityTypeConfiguration<TaskItemModel>
{
    public override void Configure(EntityTypeBuilder<TaskItemModel> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(TaskItemModel));

        builder.HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(x => x.Milestone)
            .WithMany()
            .HasForeignKey(x => x.MilestoneId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Iteration)
            .WithMany()
            .HasForeignKey(x => x.IterationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Priority)
            .WithMany()
            .HasForeignKey(x => x.PriorityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Estimate)
            .WithMany()
            .HasForeignKey(x => x.EstimateId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Status)
            .WithMany()
            .HasForeignKey(x => x.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ClosedBy)
            .WithMany()
            .HasForeignKey(x => x.ClosedById)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.ParentTaskItem)
            .WithMany(x => x.SubTaskItems)
            .HasForeignKey(x => x.ParentTaskItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.RequireAllSubsToBeComplete).IsRequired();
    }
}

public class TaskItemAssigneeModelConfiguration : ModelBaseConfiguration<TaskItemAssigneeModel>, IEntityTypeConfiguration<TaskItemAssigneeModel>
{
    public override void Configure(EntityTypeBuilder<TaskItemAssigneeModel> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(TaskItemAssigneeModel));

        builder.HasOne(x => x.TaskItem)
            .WithMany(x => x.Assignees)
            .HasForeignKey(x => x.TaskItemId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(x => x.Member)
            .WithMany()
            .HasForeignKey(x => x.MemberId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}

public class TaskItemLabelModelConfiguration : ModelBaseConfiguration<TaskItemLabelModel>, IEntityTypeConfiguration<TaskItemLabelModel>
{
    public override void Configure(EntityTypeBuilder<TaskItemLabelModel> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(TaskItemLabelModel));

        builder.HasOne(x => x.TaskItem)
            .WithMany(x => x.Labels)
            .HasForeignKey(x => x.TaskItemId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(x => x.Label)
            .WithMany()
            .HasForeignKey(x => x.LabelId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}

public class TaskItemReactionModelConfiguration : ReactionModelBaseConfiguration<TaskItemReactionModel>, IEntityTypeConfiguration<TaskItemReactionModel>
{
    public override void Configure(EntityTypeBuilder<TaskItemReactionModel> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(TaskItemReactionModel));

        builder.HasOne(x => x.TaskItem)
            .WithMany(x => x.Reactions)
            .HasForeignKey(x => x.TaskItemId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(x => x.Author)
            .WithMany()
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}

public class TaskItemCommentModelConfiguration : CommentModelBaseConfiguration<TaskItemCommentModel>, IEntityTypeConfiguration<TaskItemCommentModel>
{
    public override void Configure(EntityTypeBuilder<TaskItemCommentModel> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(TaskItemCommentModel));


        builder.HasOne(x => x.TaskItem)
            .WithMany(x => x.Comments)
            .HasForeignKey(x => x.TaskItemId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(x => x.Author)
            .WithMany()
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(x => x.ParentComment)
            .WithMany()
            .HasForeignKey(x => x.ParentCommentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class TaskItemCommentReactionModelConfiguration : ReactionModelBaseConfiguration<TaskItemCommentReactionModel>, IEntityTypeConfiguration<TaskItemCommentReactionModel>
{
    public override void Configure(EntityTypeBuilder<TaskItemCommentReactionModel> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(TaskItemCommentReactionModel));

        builder.HasOne(x => x.TaskItemComment)
            .WithMany(x => x.Reactions)
            .HasForeignKey(x => x.TaskItemCommentId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(x => x.Author)
            .WithMany()
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}

public class TaskItemBlockageModelConfiguration : ModelBaseConfiguration<TaskItemBlockageModel>, IEntityTypeConfiguration<TaskItemBlockageModel>
{
    public override void Configure(EntityTypeBuilder<TaskItemBlockageModel> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(TaskItemBlockageModel));

        builder.HasOne(x => x.BlockingTaskItem)
            .WithMany(x => x.BlockingItems)
            .HasForeignKey(x => x.BlockingTaskItemId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(x => x.BlockedTaskItem)
            .WithMany(x => x.BlockedByItems)
            .HasForeignKey(x => x.BlockedTaskItemId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}

public class TaskItemRelationModelConfiguration : ModelBaseConfiguration<TaskItemRelationModel>, IEntityTypeConfiguration<TaskItemRelationModel>
{
    public override void Configure(EntityTypeBuilder<TaskItemRelationModel> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(TaskItemBlockageModel));

        builder.HasOne(x => x.RelatedTaskItem)
            .WithMany(x => x.RelatedTaskItems)
            .HasForeignKey(x => x.RelatedTaskItemId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(x => x.RelatedToTaskItem)
            .WithMany(x => x.RelatedToTaskItems)
            .HasForeignKey(x => x.RelatedTaskItemId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}

