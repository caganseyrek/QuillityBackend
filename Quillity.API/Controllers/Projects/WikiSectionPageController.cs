using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quillity.Application.Projects.WikiSections.Pages.Commands;
using Quillity.Application.Projects.WikiSections.Pages.Queries;

namespace Quillity.API.Controllers.Projects;

[Authorize]
[ApiController]
[Route("workspaces/{workspaceId}/projects/{projectId}/wiki/{wikiSectionId}/pages")]
public class WikiSectionPageController : ControllerBase
{
    private readonly IMediator _mediator;

    public WikiSectionPageController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet()]
    public async Task<IActionResult> GetWikiSectionPagesAsync(Guid workspaceId, Guid projectId, Guid wikiSectionId)
    {
        var wikiSectionPages = await _mediator.Send(new GetWikiSectionPagesQuery
        {
            WorkspaceId = workspaceId,
            ProjectId = projectId,
            WikiSectionId = wikiSectionId,
        });
        return StatusCode(Common.Response.StatusCodes.OK, wikiSectionPages);
    }

    [HttpGet("{pageId}")]
    public async Task<IActionResult> GetWikiSectionPageByIdAsync(Guid workspaceId, Guid projectId, Guid wikiSectionId, Guid pageId)
    {
        var wikiSectionPage = await _mediator.Send(new GetWikiSectionPageByIdQuery
        {
            WorkspaceId = workspaceId,
            ProjectId = projectId,
            WikiSectionId = wikiSectionId,
            PageId = pageId,
        });
        return StatusCode(Common.Response.StatusCodes.OK, wikiSectionPage);
    }

    [HttpPost()]
    public async Task<IActionResult> CreateWikiSectionPageAsync(
         Guid workspaceId,
         Guid projectId,
         Guid wikiSectionId,
         [FromBody] CreateWikiSectionPageCommandBody createWikiSectionPageCommandBody)
    {
        await _mediator.Send(new CreateWikiSectionPageCommand
        {
            WorkspaceId = workspaceId,
            ProjectId = projectId,
            WikiSectionId = wikiSectionId,
            Title = createWikiSectionPageCommandBody.Title,
            Content = createWikiSectionPageCommandBody.Content,
        });
        return StatusCode(Common.Response.StatusCodes.CREATED, "Wiki section page created successfully");
    }

    [HttpPatch("{pageId}")]
    public async Task<IActionResult> UpdateWikiSectionPageAsync(
        Guid workspaceId,
        Guid projectId,
        Guid wikiSectionId,
        Guid pageId,
        [FromBody] UpdateWikiSectionPageCommandBody updateWikiSectionPageCommandBody)
    {
        await _mediator.Send(new UpdateWikiSectionPageCommand
        {
            WorkspaceId = workspaceId,
            ProjectId = projectId,
            WikiSectionId = wikiSectionId,
            PageId = pageId,
            Title = updateWikiSectionPageCommandBody.Title,
            Content = updateWikiSectionPageCommandBody.Content,
        });
        return StatusCode(Common.Response.StatusCodes.OK, "Wiki section page updated successfully");
    }

    [HttpDelete("{pageId}")]
    public async Task<IActionResult> DeleteWikiSectionPageAsync(Guid workspaceId, Guid projectId, Guid wikiSectionId, Guid pageId)
    {
        await _mediator.Send(new DeleteWikiSectionPageCommand
        {
            WorkspaceId = workspaceId,
            ProjectId = projectId,
            WikiSectionId = wikiSectionId,
            PageId = pageId,
        });
        return StatusCode(Common.Response.StatusCodes.OK, "Wiki section page deleted successfully");
    }
}
