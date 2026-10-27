using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quillity.Application.Workspaces.Labels.Commands;
using Quillity.Application.Workspaces.Labels.Queries;

namespace Quillity.API.Controllers.Workspaces;

[Authorize]
[ApiController]
[Route("workspaces/{workspaceId}/labels")]
public class LabelController : ControllerBase
{
    private readonly IMediator _mediator;

    public LabelController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetLabelsAsync(Guid workspaceId)
    {
        var labels = await _mediator.Send(new GetLabelsQuery
        {
            WorkspaceId = workspaceId,
        });
        return StatusCode(Common.Response.StatusCodes.OK, labels);
    }

    [HttpGet("{labelId}")]
    public async Task<IActionResult> GetLabelByIdAsync(Guid workspaceId, Guid labelId)
    {
        var label = await _mediator.Send(new GetLabelByIdQuery
        {
            WorkspaceId = workspaceId,
            LabelId = labelId,
        });
        return StatusCode(Common.Response.StatusCodes.OK, label);
    }

    [HttpPost]
    public async Task<IActionResult> CreateLabelAsync(
        Guid workspaceId,
        [FromBody] CreateLabelCommandBody createLabelCommandBody)
    {
        await _mediator.Send(new CreateLabelCommand
        {
            WorkspaceId = workspaceId,
            Name = createLabelCommandBody.Name,
            ColorCode = createLabelCommandBody.ColorCode,
            Description = createLabelCommandBody.Description,
        });
        return StatusCode(Common.Response.StatusCodes.CREATED, "Label created successfully");
    }

    [HttpPatch("{labelId}")]
    public async Task<IActionResult> UpdateLabelAsync(
        Guid workspaceId,
        Guid labelId,
        [FromBody] UpdateLabelCommandBody updateLabelCommandBody)
    {
        await _mediator.Send(new UpdateLabelCommand
        {
            WorkspaceId = workspaceId,
            LabelId = labelId,
            Name = updateLabelCommandBody.Name,
            ColorCode = updateLabelCommandBody.ColorCode,
            Description = updateLabelCommandBody.Description,
        });
        return StatusCode(Common.Response.StatusCodes.OK, "Label updated successfully");
    }

    [HttpDelete("{labelId}")]
    public async Task<IActionResult> DeleteLabelAsync(Guid workspaceId, Guid labelId)
    {
        await _mediator.Send(new DeleteLabelCommand
        {
            WorkspaceId = workspaceId,
            LabelId = labelId,
        });
        return StatusCode(Common.Response.StatusCodes.OK, "Label deleted successfully");
    }
}
