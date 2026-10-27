using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Common.Context;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Infrastructure;

namespace Quillity.Application.User.NotepadPage.Commands;

public class UpdateNotepadPageCommandBody
{
    public required string Title { get; set; }
    public required string Content { get; set; }
}

public class UpdateNotepadPageCommand : UpdateNotepadPageCommandBody, IRequest<NotepadPageDTO>
{
    public required Guid PageId { get; set; }
}

public class UpdateNotepadPageCommandHandler: IRequestHandler<UpdateNotepadPageCommand, NotepadPageDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly UserContext _userContext;

    public UpdateNotepadPageCommandHandler(QuillityDbContext dbContext, UserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<NotepadPageDTO> Handle(UpdateNotepadPageCommand request, CancellationToken cancellationToken)
    {
        var existingPage = await _dbContext.NotepadPages
            .FirstOrDefaultAsync(page =>
                StringHelper.IsEqualIgnoreCase(page.Id, request.PageId) &&
                StringHelper.IsEqualIgnoreCase(page.OwnerId, _userContext.User.Id), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.NotepadPage));

        var pageWithSameTitle = await _dbContext.NotepadPages
            .AnyAsync(page =>
                StringHelper.IsEqualIgnoreCase(page.OwnerId, _userContext.User.Id) &&
                StringHelper.IsEqualIgnoreCase(page.Title, request.Title), cancellationToken);
        if (pageWithSameTitle)
            throw new QuillityException(
                StatusCodes.BAD_REQUEST,
                ErrorMessages.GetEntryWithSamePropExistsError(ErrorMessageModel.NotepadPage, "title"));
        
        existingPage.Title = request.Title;
        existingPage.Content = request.Content;

        _dbContext.NotepadPages.Update(existingPage);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToNotepadPageDTO(existingPage);
    }
}
