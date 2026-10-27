using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quillity.Application.Workspaces.Estimates.Commands;
using Quillity.Application.Workspaces.Estimates.Queries;

namespace Quillity.API.Controllers.Workspaces;

[Authorize]
[ApiController]
[Route("workspaces/{workspaceId}/estimates")]
public class EstimateController : ControllerBase
{
    private readonly IMediator _mediator;

    public EstimateController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetEstimatesAsync(Guid workspaceId)
    {
        var estimates = await _mediator.Send(new GetEstimatesQuery
        {
            WorkspaceId = workspaceId,
        });
        return StatusCode(Common.Response.StatusCodes.OK, estimates);
    }

    [HttpGet("{estimateId}")]
    public async Task<IActionResult> GetEstimateByIdAsync(Guid workspaceId, Guid estimateId)
    {
        var estimate = await _mediator.Send(new GetEstimateByIdQuery
        {
            WorkspaceId = workspaceId,
            EstimateId = estimateId,
        });
        return StatusCode(Common.Response.StatusCodes.OK, estimate);
    }

    [HttpPost]
    public async Task<IActionResult> CreateEstimateAsync(
        Guid workspaceId,
        [FromBody] CreateEstimateCommandBody createEstimateCommandBody)
    {
        await _mediator.Send(new CreateEstimateCommand
        {
            WorkspaceId = workspaceId,
            Name = createEstimateCommandBody.Name
        });
        return StatusCode(Common.Response.StatusCodes.CREATED, "Estimate created successfully.");
    }

    [HttpPatch("{estimateId}")]
    public async Task<IActionResult> UpdateEstimateAsync(
        Guid workspaceId,
        Guid estimateId,
        [FromBody] UpdateEstimateCommandBody updateEstimateCommandBody)
    {
        await _mediator.Send(new UpdateEstimateCommand
        {
            WorkspaceId = workspaceId,
            EstimateId = estimateId,
            Name = updateEstimateCommandBody.Name
        });
        return StatusCode(Common.Response.StatusCodes.OK, "Estimate updated successfully.");
    }

    [HttpDelete("{estimateId}")]
    public async Task<IActionResult> DeleteEstimateAsync(Guid workspaceId, Guid estimateId)
    {
        await _mediator.Send(new DeleteEstimateCommand
        {
            WorkspaceId = workspaceId,
            EstimateId = estimateId
        });
        return StatusCode(Common.Response.StatusCodes.OK, "Estimate deleted successfully.");
    }

    [HttpPost("{estimateId}/items")]
    public async Task<IActionResult> CreateEstimateItemAsync(
        Guid workspaceId,
        Guid estimateId,
        [FromBody] CreateEstimateItemCommandBody createEstimateItemCommandBody)
    {
        await _mediator.Send(new CreateEstimateItemCommand
        {
            WorkspaceId = workspaceId,
            EstimateId = estimateId,
            Label = createEstimateItemCommandBody.Label,
            MagnitudeIndex = createEstimateItemCommandBody.MagnitudeIndex,
        });
        return StatusCode(Common.Response.StatusCodes.CREATED, "Estimate item created successfully.");
    }

    [HttpPatch("{estimateId}/items/{estimateItemId}")]
    public async Task<IActionResult> UpdateEstimateItemAsync(
        Guid workspaceId,
        Guid estimateId,
        Guid estimateItemId,
        [FromBody] UpdateEstimateItemCommandBody updateEstimateItemCommandBody)
    {
        await _mediator.Send(new UpdateEstimateItemCommand
        {
            WorkspaceId = workspaceId,
            EstimateId = estimateId,
            EstimateItemId = estimateItemId,
            Label = updateEstimateItemCommandBody.Label,
            MagnitudeIndex = updateEstimateItemCommandBody.MagnitudeIndex,
        });
        return StatusCode(Common.Response.StatusCodes.OK, "Estimate item updated successfully.");
    }

    [HttpDelete("{estimateId}/items/{estimateItemId}")]
    public async Task<IActionResult> DeleteEstimateItemAsync(Guid workspaceId, Guid estimateId, Guid estimateItemId)
    {
        await _mediator.Send(new DeleteEstimateItemCommand
        {
            WorkspaceId = workspaceId,
            EstimateId = estimateId,
            EstimateItemId = estimateItemId,
        });
        return StatusCode(Common.Response.StatusCodes.OK, "Estimate item deleted successfully.");
    }
}
