using Quillity.Common.Models;
using Quillity.Common.Values;

namespace Quillity.Common.Presets;

public class ProjectPresets
{
    protected readonly ProjectModel _project;

    public ProjectPresets(ProjectModel project)
    {
        _project = project;
    }

    public ProjectRoleModel GetLeadRole() => new()
    {
        ProjectId = _project.Id,
        Project = _project,
        Name = "Owner",
        HierarchyIndex = 0,
        Permissions = PermissionsPerRole.ProjectLeadPermissions,
    };

    public ProjectRoleModel GetContributorRole() => new()
    {
        ProjectId = _project.Id,
        Project = _project,
        Name = "Admin",
        HierarchyIndex = 1,
        Permissions = PermissionsPerRole.ProjectContributorPermissions,
    };
}
