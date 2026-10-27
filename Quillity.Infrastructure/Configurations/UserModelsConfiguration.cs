using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quillity.Common.Models;

namespace Quillity.Infrastructure.Configurations;

public class UserModelConfiguration : ModelBaseConfiguration<UserModel>, IEntityTypeConfiguration<UserModel>
{
    public override void Configure(EntityTypeBuilder<UserModel> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(UserModel));
        builder.Property(x => x.FullName).IsRequired();
        builder.Property(x => x.UserName).IsRequired();
        builder.Property(x => x.Email).IsRequired();
        builder.Property(x => x.EmailVerificationToken);
        builder.Property(x => x.EmailVerificationTokenExpirationInMs);
        builder.Property(x => x.EmailVerified).IsRequired();
        builder.Property(x => x.PasswordHash).IsRequired();
        builder.Property(x => x.PasswordResetToken);
        builder.Property(x => x.PasswordResetTokenExpirationInMs);
        builder.Property(x => x.ProfilePictureUrl);
        builder.Property(x => x.InitialsForFallback).IsRequired();
        builder.Property(x => x.IsSuspended).IsRequired();
    }
}

public class UserPreferencesModelConfiguration : ModelBaseConfiguration<UserPreferencesModel>, IEntityTypeConfiguration<UserPreferencesModel>
{
    public override void Configure(EntityTypeBuilder<UserPreferencesModel> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(UserPreferencesModel));

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(x => x.PreferredUiTheme).IsRequired();
        builder.Property(x => x.UserTimeZone).IsRequired();    
        builder.Property(x => x.FirstDayOfWeek).IsRequired();    
        builder.Property(x => x.DateFormat).IsRequired();    
        builder.Property(x => x.TimeFormat).IsRequired();    
        builder.Property(x => x.Use24HourClock).IsRequired();    
        //builder.Property(x => x.PreferUserNameOverFullName).IsRequired();    
        //builder.Property(x => x.PreferEmojisOverTextEmoticons).IsRequired();    
        builder.Property(x => x.AutoAssignNewTasksToSelf).IsRequired();    
    }
}

public class UserInvitationModelConfiguration : ModelBaseConfiguration<InvitationModel>, IEntityTypeConfiguration<InvitationModel>
{
    public override void Configure(EntityTypeBuilder<InvitationModel> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(InvitationModel));

        builder.HasOne(x => x.InvitedWorkspace)
            .WithMany()
            .HasForeignKey(x => x.InvitedWorkspaceId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(x => x.InvitedByUser)
            .WithMany()
            .HasForeignKey(x => x.InvitedByUserId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(x => x.InvitedUser)
            .WithMany()
            .HasForeignKey(x => x.InvitedUserId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.Property(x => x.InvitationNote).IsRequired();
        builder.Property(x => x.InvitedAtMs).IsRequired();
        builder.Property(x => x.AcceptedAtMs);
    }
}

public class NotepadPageModelConfiguration : PageModelBaseConfiguration<NotepadPageModel>, IEntityTypeConfiguration<NotepadPageModel>
{
    public override void Configure(EntityTypeBuilder<NotepadPageModel> builder)
    {
        base.Configure(builder);
        builder.ToTable(nameof(NotepadPageModel));

        builder.HasOne(x => x.Owner)
            .WithMany()
            .HasForeignKey(x => x.OwnerId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
