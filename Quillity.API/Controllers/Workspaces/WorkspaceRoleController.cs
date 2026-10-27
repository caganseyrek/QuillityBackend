using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quillity.Application.Workspaces.Roles.Commands;
using Quillity.Application.Workspaces.Roles.Queries;

namespace Quillity.API.Controllers.Workspaces;

[Authorize]
[ApiController]
[Route("workspaces/{workspaceId}/roles")]
public class WorkspaceRoleController : ControllerBase
{
    private readonly IMediator _mediator;

    public WorkspaceRoleController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetWorkspaceRolesAsync(Guid workspaceId)
    {
        var roles = await _mediator.Send(new GetWorkspaceRolesQuery
        {
            WorkspaceId = workspaceId,
        });
        return StatusCode(Common.Response.StatusCodes.OK, roles);
    }

    [HttpGet("{roleId}")]
    public async Task<IActionResult> GetWorkspaceRoleByIdAsync(Guid workspaceId, Guid roleId)
    {
        var roles = await _mediator.Send(new GetWorkspaceRoleByIdQuery
        {
            WorkspaceId = workspaceId,
            RoleId = roleId,
        });
        return StatusCode(Common.Response.StatusCodes.OK, roles);
    }

    [HttpPost]
    public async Task<IActionResult> CreateWorkspaceRoleAsync(
        Guid workspaceId,
        [FromBody] CreateWorkspaceRoleCommandBody createWorkspaceRoleCommandBody)
    {
        await _mediator.Send(new CreateWorkspaceRoleCommand
        {
            WorkspaceId = workspaceId,
            Name = createWorkspaceRoleCommandBody.Name,
            HierarchyIndex = createWorkspaceRoleCommandBody.HierarchyIndex,
            Permissions = createWorkspaceRoleCommandBody.Permissions,
        });
        return StatusCode(Common.Response.StatusCodes.CREATED, "Workspace role created successfully");
    }

    [HttpPatch("{roleId}")]
    public async Task<IActionResult> UpdateWorkspaceRoleAsync(
        Guid workspaceId,
        Guid roleId,
        [FromBody] UpdateWorkspaceRoleCommandBody updateWorkspaceRoleCommandBody)
    {
        await _mediator.Send(new UpdateWorkspaceRoleCommand
        {
            WorkspaceId = workspaceId,
            RoleId = roleId,
            Name = updateWorkspaceRoleCommandBody.Name,
            HierarchyIndex = updateWorkspaceRoleCommandBody.HierarchyIndex,
            Permissions = updateWorkspaceRoleCommandBody.Permissions,
        });
        return StatusCode(Common.Response.StatusCodes.OK, "Workspace role updated successfully");
    }

    [HttpDelete("{roleId}")]
    public async Task<IActionResult> DeleteWorkspaceRoleAsync(Guid workspaceId, Guid roleId)
    {
        await _mediator.Send(new DeleteWorkspaceRoleCommand
        {
            WorkspaceId = workspaceId,
            RoleId = roleId,
        });
        return StatusCode(Common.Response.StatusCodes.OK, "Workspace role deleted successfully");
    }
}
