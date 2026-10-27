using MediatR;
using Quillity.Application.DTOs;
using Quillity.Common.Context;
using Quillity.Common.Helpers;
using Quillity.Infrastructure;

namespace Quillity.Application.User.NotepadPage.Queries;

public class GetNotepadPagesQuery : IRequest<List<NotepadPageDTO>>
{
}

public class GetNotepadPagesQueryHandler : IRequestHandler<GetNotepadPagesQuery, List<NotepadPageDTO>>
{
    private readonly QuillityDbContext _dbContext;
    private readonly UserContext _userContext;

    public GetNotepadPagesQueryHandler(QuillityDbContext dbContext, UserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public Task<List<NotepadPageDTO>> Handle(GetNotepadPagesQuery request, CancellationToken cancellationToken)
    {
        var notepadPages = _dbContext.NotepadPages
            .Where(page => StringHelper.IsEqualIgnoreCase(page.OwnerId, _userContext.User.Id))
            .Select(ObjectMapper.ToNotepadPageDTO)
            .ToList();

        return Task.FromResult(notepadPages);
    }
}