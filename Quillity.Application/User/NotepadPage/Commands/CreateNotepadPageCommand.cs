using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Common.Context;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Models;
using Quillity.Common.Response;
using Quillity.Infrastructure;

namespace Quillity.Application.User.NotepadPage.Commands;

public class CreateNotepadPageCommand : IRequest<NotepadPageDTO>
{
    public required string Title { get; set; }
    public required string Content { get; set; }
}

public class CreateNotepadPageCommandHandler : IRequestHandler<CreateNotepadPageCommand, NotepadPageDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly UserContext _userContext;

    public CreateNotepadPageCommandHandler(QuillityDbContext dbContext, UserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<NotepadPageDTO> Handle(CreateNotepadPageCommand request, CancellationToken cancellationToken)
    {
        var pageWithSameTitle = await _dbContext.NotepadPages
            .AnyAsync(page =>
                StringHelper.IsEqualIgnoreCase(page.OwnerId, _userContext.User.Id) &&
                StringHelper.IsEqualIgnoreCase(page.Title, request.Title), cancellationToken);
        if (pageWithSameTitle)
            throw new QuillityException(
                StatusCodes.BAD_REQUEST,
                ErrorMessages.GetEntryWithSamePropExistsError(ErrorMessageModel.NotepadPage, "title"));

        var newPage = new NotepadPageModel
        {
            OwnerId = _userContext.User.Id,
            Owner = _userContext.User,
            Title = request.Title,
            Content = request.Content,
        };

        await _dbContext.NotepadPages.AddAsync(newPage, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToNotepadPageDTO(newPage);
    }
}
