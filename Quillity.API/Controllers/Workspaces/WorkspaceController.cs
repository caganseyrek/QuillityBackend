using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quillity.Application.Workspaces.Core.Commands;
using Quillity.Application.Workspaces.Core.Queries;

namespace Quillity.API.Controllers.Workspaces;

[Authorize]
[ApiController]
[Route("workspaces")]
public class WorkspaceController : ControllerBase
{
    private readonly IMediator _mediator;

    public WorkspaceController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetWorkspacesAsync()
    {
        var workspaces = await _mediator.Send(new GetWorkspacesQuery());
        return StatusCode(Common.Response.StatusCodes.OK, workspaces);
    }

    [HttpGet("{workspaceId}")]
    public async Task<IActionResult> GetWorkspaceByIdAsync(Guid workspaceId)
    {
        var workspace = await _mediator.Send(new GetWorkspaceByIdQuery { WorkspaceId = workspaceId });
        return StatusCode(Common.Response.StatusCodes.OK, workspace);
    }

    [HttpPost]
    public async Task<IActionResult> CreateWorkspaceAsync([FromBody] CreateWorkspaceCommand createWorkspaceCommand)
    {
        await _mediator.Send(createWorkspaceCommand);
        return StatusCode(Common.Response.StatusCodes.CREATED, "Workspace created successfully");
    }

    [HttpPatch("{workspaceId}")]
    public async Task<IActionResult> UpdateWorkspaceAsync(
        Guid workspaceId,
        [FromBody] UpdateWorkspaceCommandBody updateWorkspaceCommandBody)
    {
        await _mediator.Send(new UpdateWorkspaceCommand
        {
            WorkspaceId = workspaceId,
            Name = updateWorkspaceCommandBody.Name,
            Description = updateWorkspaceCommandBody.Description,
            UrlKey = updateWorkspaceCommandBody.UrlKey,
            LogoImageUrl = updateWorkspaceCommandBody.LogoImageUrl,
        });
        return StatusCode(Common.Response.StatusCodes.OK, "Workspace updated successfully");
    }

    [HttpDelete("{workspaceId}")]
    public async Task<IActionResult> DeleteWorkspaceAsync(Guid workspaceId)
    {
        await _mediator.Send(new DeleteWorkspaceCommand { WorkspaceId = workspaceId });
        return StatusCode(Common.Response.StatusCodes.OK, "Workspace deleted successfully");
    }
}
