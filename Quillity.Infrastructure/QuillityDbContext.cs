using Microsoft.EntityFrameworkCore;
using Quillity.Common.Models;
using System.Reflection;

namespace Quillity.Infrastructure;

public class QuillityDbContext : DbContext
{
    public DbSet<WorkspaceModel> Workspaces { get; set; }
    public DbSet<WorkspaceRoleModel> WorkspaceRoles { get; set; }
    public DbSet<WorkspaceMemberModel> WorkspaceMembers { get; set; }
    public DbSet<StatusModel> Statuses { get; set; }
    public DbSet<PriorityModel> Priorities { get; set; }
    public DbSet<LabelModel> Labels { get; set; }
    public DbSet<EstimateModel> Estimates { get; set; }
    public DbSet<EstimateItemModel> EstimateItems { get; set; }

    public DbSet<ProjectModel> Projects { get; set; }
    public DbSet<ProjectRoleModel> ProjectRoles { get; set; }
    public DbSet<ProjectMemberModel> ProjectMembers { get; set; }
    public DbSet<MilestoneModel> Milestones { get; set; }
    public DbSet<IterationModel> Iterations { get; set; }
    public DbSet<HealthUpdateModel> HealthUpdates { get; set; }
    public DbSet<HealthUpdateReactionModel> HealthUpdateReactions { get; set; }
    public DbSet<HealthUpdateCommentModel> HealthUpdateComments { get; set; }
    public DbSet<HealthUpdateCommentReactionModel> HealthUpdateCommentReactions { get; set; }
    public DbSet<WikiSectionModel> WikiSections { get; set; }
    public DbSet<WikiSectionPageModel> WikiSectionPages { get; set; }

    public DbSet<TaskItemModel> TaskItems { get; set; }
    public DbSet<TaskItemAssigneeModel> TaskItemAssignees { get; set; }
    public DbSet<TaskItemLabelModel> TaskItemLabels { get; set; }
    public DbSet<TaskItemReactionModel> TaskItemReactions { get; set; }
    public DbSet<TaskItemCommentModel> TaskItemComments { get; set; }
    public DbSet<TaskItemCommentReactionModel> TaskItemCommentReactions { get; set; }
    public DbSet<TaskItemBlockageModel> TaskItemBlockages { get; set; }
    public DbSet<TaskItemRelationModel> TaskItemRelations { get; set; }

    public DbSet<UserModel> Users { get; set; }
    public DbSet<UserPreferencesModel> UserPreferences { get; set; }
    public DbSet<NotepadPageModel> NotepadPages { get; set; }
    public DbSet<InvitationModel> Invitations { get; set; }
    public DbSet<UserSessionModel> UserSessions { get; set; }
    public DbSet<UserCreatedApiKeyModel> UserCreatedAPIKeys { get; set; }
    public DbSet<ActivityLogModel> ActivityLogs { get; set; }

    public QuillityDbContext(DbContextOptions<QuillityDbContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
