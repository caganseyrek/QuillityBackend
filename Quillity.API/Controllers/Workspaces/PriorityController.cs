using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quillity.Application.Workspaces.Priorities.Commands;
using Quillity.Application.Workspaces.Priorities.Queries;

namespace Quillity.API.Controllers.Workspaces;

[Authorize]
[ApiController]
[Route("workspaces/{workspaceId}/priorities")]
public class PriorityController : ControllerBase
{
    private readonly IMediator _mediator;

    public PriorityController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetPrioritiesAsync(Guid workspaceId)
    {
        var priorities = await _mediator.Send(new GetPrioritiesQuery
        {
            WorkspaceId = workspaceId,
        });
        return StatusCode(Common.Response.StatusCodes.OK, priorities);
    }

    [HttpGet("{priorityId}")]
    public async Task<IActionResult> GetPriorityByIdAsync(Guid workspaceId, Guid priorityId)
    {
        var priority = await _mediator.Send(new GetPriorityByIdQuery
        {
            WorkspaceId = workspaceId,
            PriorityId = priorityId,
        });
        return StatusCode(Common.Response.StatusCodes.OK, priority);
    }

    [HttpPost]
    public async Task<IActionResult> CreatePriorityAsync(
        Guid workspaceId,
        [FromBody] CreatePriorityCommandBody createPriorityCommandBody)
    {
        await _mediator.Send(new CreatePriorityCommand
        {
            WorkspaceId = workspaceId,
            Name = createPriorityCommandBody.Name,
            ColorCode = createPriorityCommandBody.ColorCode,
            MagnitudeIndex = createPriorityCommandBody.MagnitudeIndex,
        });
        return StatusCode(Common.Response.StatusCodes.CREATED, "Priority created successfully");
    }

    [HttpPatch("{priorityId}")]
    public async Task<IActionResult> UpdatePriorityAsync(
        Guid workspaceId,
        Guid priorityId,
        [FromBody] UpdatePriorityCommandBody updatePriorityCommandBody)
    {
        await _mediator.Send(new UpdatePriorityCommand
        {
            WorkspaceId = workspaceId,
            PriorityId = priorityId,
            Name = updatePriorityCommandBody.Name,
            ColorCode = updatePriorityCommandBody.ColorCode,
            MagnitudeIndex = updatePriorityCommandBody.MagnitudeIndex,
        });
        return StatusCode(Common.Response.StatusCodes.OK, "Priority updated successfully");
    }

    [HttpDelete("{priorityId}")]
    public async Task<IActionResult> DeletePriorityAsync(Guid workspaceId, Guid priorityId)
    {
        await _mediator.Send(new DeletePriorityCommand
        {
            WorkspaceId = workspaceId,
            PriorityId = priorityId,
        });
        return StatusCode(Common.Response.StatusCodes.OK, "Priority deleted successfully");
    }
}
