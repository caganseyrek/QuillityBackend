using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quillity.Application.Workspaces.Statuses.Commands;
using Quillity.Application.Workspaces.Statuses.Queries;

namespace Quillity.API.Controllers.Workspaces;

[Authorize]
[ApiController]
[Route("workspaces/{workspaceId}/statuses")]
public class StatusController : ControllerBase
{
    private readonly IMediator _mediator;

    public StatusController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetStatusesAsync(Guid workspaceId)
    {
        var statuses = await _mediator.Send(new GetStatusesQuery
        {
            WorkspaceId = workspaceId
        });
        return StatusCode(Common.Response.StatusCodes.OK, statuses);
    }

    [HttpGet("{statusId}")]
    public async Task<IActionResult> GetStatusByIdAsync(Guid workspaceId, Guid statusId)
    {
        var status = await _mediator.Send(new GetStatusByIdQuery
        {
            WorkspaceId = workspaceId,
            StatusId = statusId,
        });
        return StatusCode(Common.Response.StatusCodes.OK, status);
    }

    [HttpPost]
    public async Task<IActionResult> CreateStatusAsync(
        Guid workspaceId,
        [FromBody] CreateStatusCommandBody createStatusCommandBody)
    {
        await _mediator.Send(new CreateStatusCommand
        {
            WorkspaceId = workspaceId,
            Name = createStatusCommandBody.Name,
            ColorCode = createStatusCommandBody.ColorCode,
            Group = createStatusCommandBody.Group,
        });
        return StatusCode(Common.Response.StatusCodes.CREATED, "Status created successfully");
    }

    [HttpPatch("{statusId}")]
    public async Task<IActionResult> UpdateStatusAsync(
        Guid workspaceId,
        Guid statusId,
        [FromBody] UpdateStatusCommandBody updateStatusCommandBody)
    {
        await _mediator.Send(new UpdateStatusCommand
        {
            WorkspaceId = workspaceId,
            StatusId = statusId,
            Name = updateStatusCommandBody.Name,
            ColorCode = updateStatusCommandBody.ColorCode,
            Group = updateStatusCommandBody.Group,
        });
        return StatusCode(Common.Response.StatusCodes.OK, "Status updated successfully");
    }

    [HttpDelete("{statusId}")]
    public async Task<IActionResult> DeleteStatusAsync(Guid workspaceId, Guid statusId)
    {
        await _mediator.Send(new DeleteStatusCommand
        {
            WorkspaceId = workspaceId,
            StatusId = statusId,
        });
        return StatusCode(Common.Response.StatusCodes.OK, "Status deleted successfully");
    }
}
