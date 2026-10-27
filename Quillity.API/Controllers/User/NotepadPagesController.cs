using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quillity.Application.User.NotepadPage.Queries;
using Quillity.Application.User.NotepadPage.Commands;

namespace Quillity.API.Controllers.User;

[Authorize]
[ApiController]
[Route("user/notepad")]
public class NotepadPagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotepadPagesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotepadPagesAsync()
    {
        var notepadPages = await _mediator.Send(new GetNotepadPagesQuery());
        return StatusCode(Common.Response.StatusCodes.OK, notepadPages);
    }

    [HttpGet("{pageId}")]
    public async Task<IActionResult> GetNotepadPageByIdAsync(Guid pageId)
    {
        var notepadPages = await _mediator.Send(new GetNotepadPageByIdQuery
        {
            PageId = pageId,
        });
        return StatusCode(Common.Response.StatusCodes.OK, notepadPages);
    }

    [HttpPost]
    public async Task<IActionResult> CreateNotepadPageAsync([FromBody] CreateNotepadPageCommand createNotepadPageCommand)
    {
        await _mediator.Send(createNotepadPageCommand);
        return StatusCode(Common.Response.StatusCodes.OK, "Notepad page created successfully");
    }

    [HttpPatch("{pageId}")]
    public async Task<IActionResult> UpdateNotepadPageAsync(
        Guid pageId,
        [FromBody] UpdateNotepadPageCommandBody createNotepadPageCommandBody)
    {
        await _mediator.Send(new UpdateNotepadPageCommand
        {
            PageId = pageId,
            Content = createNotepadPageCommandBody.Content,
            Title = createNotepadPageCommandBody.Title,
        });
        return StatusCode(Common.Response.StatusCodes.OK, "Notepad page updated successfully");
    }

    [HttpDelete("{pageId}")]
    public async Task<IActionResult> DeleteNotepadPageAsync(Guid pageId)
    {
        await _mediator.Send(new DeleteNotepadPageCommand
        {
            PageId = pageId,
        });
        return StatusCode(Common.Response.StatusCodes.OK, "Notepad page deleted successfully");
    }
}
