using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Common.Context;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Infrastructure;

namespace Quillity.Application.User.NotepadPage.Queries;

public class GetNotepadPageByIdQuery : IRequest<NotepadPageDTO>
{
    public Guid PageId { get; set; }
}

public class GetNotepadPageByIdQueryHandler : IRequestHandler<GetNotepadPageByIdQuery, NotepadPageDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly UserContext _userContext;

    public GetNotepadPageByIdQueryHandler(QuillityDbContext dbContext, UserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<NotepadPageDTO> Handle(GetNotepadPageByIdQuery request, CancellationToken cancellationToken)
    {
        var notepadPage = await _dbContext.NotepadPages
            .FirstOrDefaultAsync(page => 
                StringHelper.IsEqualIgnoreCase(page.Id, request.PageId) &&
                StringHelper.IsEqualIgnoreCase(page.OwnerId, _userContext.User.Id), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.NotepadPage));

        return ObjectMapper.ToNotepadPageDTO(notepadPage);
    }
}
