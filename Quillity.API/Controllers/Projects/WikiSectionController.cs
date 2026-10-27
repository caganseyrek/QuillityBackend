using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quillity.Application.Projects.WikiSections.Core.Commands;
using Quillity.Application.Projects.WikiSections.Core.Queries;

namespace Quillity.API.Controllers.Projects;

[Authorize]
[ApiController]
[Route("workspaces/{workspaceId}/projects/{projectId}/wikis")]
public class WikiController : ControllerBase
{
    private readonly IMediator _mediator;

    public WikiController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetWikiSectionsAsync(Guid workspaceId, Guid projectId)
    {
        var wikiSections = await _mediator.Send(new GetWikiSectionsQuery
        {
            WorkspaceId = workspaceId,
            ProjectId = projectId,
        });
        return StatusCode(Common.Response.StatusCodes.OK, wikiSections);
    }

    [HttpGet("{wikiSectionId}")]
    public async Task<IActionResult> GetWikiSectionByIdAsync(Guid workspaceId, Guid projectId, Guid wikiSectionId)
    {
        var wikiSection = await _mediator.Send(new GetWikiSectionByIdQuery
        {
            WorkspaceId = workspaceId,
            ProjectId = projectId,
            WikiSectionId = wikiSectionId,
        });
        return StatusCode(Common.Response.StatusCodes.OK, wikiSection);
    }

    [HttpPost]
    public async Task<IActionResult> CreateWikiSectionAsync(
        Guid workspaceId,
        Guid projectId,
        [FromBody] CreateWikiSectionCommandBody createWikiSectionCommandBody)
    {
        await _mediator.Send(new CreateWikiSectionCommand
        {
            WorkspaceId = workspaceId,
            ProjectId = projectId,
            Name = createWikiSectionCommandBody.Name,
            Description = createWikiSectionCommandBody.Description,
        });
        return StatusCode(Common.Response.StatusCodes.CREATED, "Wiki section created successfully");
    }

    [HttpPatch("{wikiSectionId}")]
    public async Task<IActionResult> UpdateWikiSectionAsync(
        Guid workspaceId,
        Guid projectId,
        Guid wikiSectionId,
        [FromBody] UpdateWikiSectionCommandBody updateWikiSectionCommandBody)
    {
        await _mediator.Send(new UpdateWikiSectionCommand
        {
            WorkspaceId = workspaceId,
            ProjectId = projectId,
            WikiSectionId = wikiSectionId,
            Name = updateWikiSectionCommandBody.Name,
            Description = updateWikiSectionCommandBody.Description,
        });
        return StatusCode(Common.Response.StatusCodes.OK, "Wiki section updated successfully");
    }

    [HttpDelete("{wikiId}")]
    public async Task<IActionResult> DeleteWikiSectionAsync(Guid workspaceId, Guid projectId, Guid wikiSectionId)
    {
        await _mediator.Send(new DeleteWikiSectionCommand
        {
            WorkspaceId = workspaceId,
            ProjectId = projectId,
            WikiSectionId = wikiSectionId,
        });
        return StatusCode(Common.Response.StatusCodes.OK, "Wiki section deleted successfully");
    }
}
