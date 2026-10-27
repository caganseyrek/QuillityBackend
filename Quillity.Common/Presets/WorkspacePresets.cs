using Quillity.Common.Models;
using Quillity.Common.Values;

namespace Quillity.Common.Presets;

public class WorkspacePresets
{
    protected readonly WorkspaceModel _workspace;

    public WorkspacePresets(WorkspaceModel workspace)
    {
        _workspace = workspace;
    }

    public WorkspaceRoleModel GetOwnerRole() => new()
    {
        WorkspaceId = _workspace.Id,
        Workspace = _workspace,
        Name = "Owner",
        HierarchyIndex = 0,
        Permissions = PermissionsPerRole.WorkspaceOwnerPermissions,
    };

    public WorkspaceRoleModel GetAdminRole() => new()
    {
        WorkspaceId = _workspace.Id,
        Workspace = _workspace,
        Name = "Admin",
        HierarchyIndex = 1,
        Permissions = PermissionsPerRole.WorkspaceAdminPermissions,
    };

    public WorkspaceRoleModel GetMemberRole() => new()
    {
        WorkspaceId = _workspace.Id,
        Workspace = _workspace,
        Name = "Member",
        HierarchyIndex = 2,
        Permissions = PermissionsPerRole.WorkspaceMemberPermissions,
    };

    public List<StatusModel> GetStatuses()
    {
        return [
            new StatusModel
            {
                Name = "Backlog",
                ColorCode = "#9B9B9B",
                Group = StatusItemGroup.Backlog,
                Workspace = _workspace,
                WorkspaceId = _workspace.Id,
            },
            new StatusModel
            {
                Name = "To Do",
                ColorCode = "#FF6B6B",
                Group = StatusItemGroup.NotStarted,
                Workspace = _workspace,
                WorkspaceId = _workspace.Id,
            },
            new StatusModel
            {
                Name = "In Progress",
                ColorCode = "#4ECDC4",
                Group = StatusItemGroup.InProgress,
                Workspace = _workspace,
                WorkspaceId = _workspace.Id,
            },
            new StatusModel
            {
                Name = "In Review",
                ColorCode = "#FFD93D",
                Group = StatusItemGroup.InProgress,
                Workspace = _workspace,
                WorkspaceId = _workspace.Id,
            },
            new StatusModel
            {
                Name = "Done",
                ColorCode = "#1A936F",
                Group = StatusItemGroup.Completed,
                Workspace = _workspace,
                WorkspaceId = _workspace.Id,
            },
            new StatusModel
            {
                Name = "Canceled",
                ColorCode = "#B00020",
                Group = StatusItemGroup.Canceled,
                Workspace = _workspace,
                WorkspaceId = _workspace.Id,
            }
        ];
    }


    public List<PriorityModel> GetPriorities()
    {
        return [
            new PriorityModel
            {
                Name = "Critical",
                ColorCode = "#D72631",
                WorkspaceId = _workspace.Id,
                Workspace = _workspace,
                MagnitudeIndex = 1
            },
            new PriorityModel
            {
                Name = "High",
                ColorCode = "#F46036",
                WorkspaceId = _workspace.Id,
                Workspace = _workspace,
                MagnitudeIndex = 2
            },
            new PriorityModel
            {
                Name = "Medium",
                ColorCode = "#2E86AB",
                WorkspaceId = _workspace.Id,
                Workspace = _workspace,
                MagnitudeIndex = 3
            },
            new PriorityModel
            {
                Name = "Low",
                ColorCode = "#6A9A82",
                WorkspaceId = _workspace.Id,
                Workspace = _workspace,
                MagnitudeIndex = 4
            },
            new PriorityModel
            {
                Name = "None",
                ColorCode = "#9B9B9B",
                WorkspaceId = _workspace.Id,
                Workspace = _workspace,
                MagnitudeIndex = 5
            }
        ];
    }

    public List<EstimateModel> GetEstimates()
    {
        var fibonacciEstimate = new EstimateModel
        {
            WorkspaceId = _workspace.Id,
            Workspace = _workspace,
            Name = "Fibonacci",
            Items = [],
        };
        fibonacciEstimate.Items = [
            new() { Estimate = fibonacciEstimate, EstimateId = fibonacciEstimate.Id, Label = "1", MagnitudeIndex = 1 },
            new() { Estimate = fibonacciEstimate, EstimateId = fibonacciEstimate.Id, Label = "2", MagnitudeIndex = 2 },
            new() { Estimate = fibonacciEstimate, EstimateId = fibonacciEstimate.Id, Label = "3", MagnitudeIndex = 3 },
            new() { Estimate = fibonacciEstimate, EstimateId = fibonacciEstimate.Id, Label = "5", MagnitudeIndex = 5 },
            new() { Estimate = fibonacciEstimate, EstimateId = fibonacciEstimate.Id, Label = "8", MagnitudeIndex = 8 },
            new() { Estimate = fibonacciEstimate, EstimateId = fibonacciEstimate.Id, Label = "13", MagnitudeIndex = 13 },
            new() { Estimate = fibonacciEstimate, EstimateId = fibonacciEstimate.Id, Label = "21", MagnitudeIndex = 21 },
        ];

        var tshirtEstimate = new EstimateModel
        {
            WorkspaceId = _workspace.Id,
            Workspace = _workspace,
            Name = "T-Shirt Sizes",
            Items = [],
        };
        tshirtEstimate.Items = [
            new() { Estimate = tshirtEstimate, EstimateId = tshirtEstimate.Id, Label = "XS", MagnitudeIndex = 1 },
            new() { Estimate = tshirtEstimate, EstimateId = tshirtEstimate.Id, Label = "S", MagnitudeIndex = 2 },
            new() { Estimate = tshirtEstimate, EstimateId = tshirtEstimate.Id, Label = "M", MagnitudeIndex = 3 },
            new() { Estimate = tshirtEstimate, EstimateId = tshirtEstimate.Id, Label = "L", MagnitudeIndex = 5 },
            new() { Estimate = tshirtEstimate, EstimateId = tshirtEstimate.Id, Label = "XL", MagnitudeIndex = 8 },
            new() { Estimate = tshirtEstimate, EstimateId = tshirtEstimate.Id, Label = "XXL", MagnitudeIndex = 13 }
        ];

        var timeEstimate = new EstimateModel
        {
            WorkspaceId = _workspace.Id,
            Workspace = _workspace,
            Name = "Hours Estimate",
            Items = [],
        };
        timeEstimate.Items = [
            new() { Estimate = timeEstimate, EstimateId = timeEstimate.Id, Label = "15m", MagnitudeIndex = 1 },
            new() { Estimate = timeEstimate, EstimateId = timeEstimate.Id, Label = "30m", MagnitudeIndex = 2 },
            new() { Estimate = timeEstimate, EstimateId = timeEstimate.Id, Label = "1h", MagnitudeIndex = 4 },
            new() { Estimate = timeEstimate, EstimateId = timeEstimate.Id, Label = "2h", MagnitudeIndex = 8 },
            new() {Estimate = timeEstimate, EstimateId = timeEstimate.Id, Label = "4h", MagnitudeIndex = 16 },
            new() {Estimate = timeEstimate, EstimateId = timeEstimate.Id, Label = "8h", MagnitudeIndex = 32 },
            new() {Estimate = timeEstimate, EstimateId = timeEstimate.Id, Label = "2d+", MagnitudeIndex = 64 },
        ];

        var pointBasedEstimate = new EstimateModel
        {
            WorkspaceId = _workspace.Id,
            Workspace = _workspace,
            Name = "Linear Points",
            Items = [],
        };
        pointBasedEstimate.Items = [
            new() {Estimate = timeEstimate, EstimateId = timeEstimate.Id, Label = "0", MagnitudeIndex = 0},
            new() {Estimate = timeEstimate, EstimateId = timeEstimate.Id, Label = "1", MagnitudeIndex = 1},
            new() {Estimate = timeEstimate, EstimateId = timeEstimate.Id, Label = "2", MagnitudeIndex = 2},
            new() {Estimate = timeEstimate, EstimateId = timeEstimate.Id, Label = "3", MagnitudeIndex = 3},
            new() {Estimate = timeEstimate, EstimateId = timeEstimate.Id, Label = "4", MagnitudeIndex = 4},
            new() {Estimate = timeEstimate, EstimateId = timeEstimate.Id, Label = "5", MagnitudeIndex = 5},
            new() {Estimate = timeEstimate, EstimateId = timeEstimate.Id, Label = "8", MagnitudeIndex = 8},
            new() { Estimate = timeEstimate, EstimateId = timeEstimate.Id, Label = "13", MagnitudeIndex = 13 },
        ];

        return [
            fibonacciEstimate,
            tshirtEstimate,
            timeEstimate,
            pointBasedEstimate,
        ];
    }

    public List<LabelModel> GetLabels()
    {
        return [
        new LabelModel
            {
                Name = "Bug",
                ColorCode = "#E63946",
                Description = "Indicates a defect or unintended behavior",
                WorkspaceId = _workspace.Id,
                Workspace = _workspace,
            },
            new LabelModel
            {
                Name = "Feature",
                ColorCode = "#1D3557",
                Description = "Represents a new capability or enhancement",
                WorkspaceId = _workspace.Id,
                Workspace = _workspace,
            },
            new LabelModel
            {
                Name = "Tech Debt",
                ColorCode = "#F4A261",
                Description = "Code or architectural improvements needed",
                WorkspaceId = _workspace.Id,
                Workspace = _workspace,
            },
            new LabelModel
            {
                Name = "Blocked",
                ColorCode = "#6A4C93",
                Description = "Work cannot proceed due to external dependencies",
                WorkspaceId = _workspace.Id,
                Workspace = _workspace,
            },
            new LabelModel
            {
                Name = "Design",
                ColorCode = "#2A9D8F",
                Description = "Tasks that involve UI/UX or visual design",
                WorkspaceId = _workspace.Id,
                Workspace = _workspace,
            }
        ];
    }
}
