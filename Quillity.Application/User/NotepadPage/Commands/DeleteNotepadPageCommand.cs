using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Common.Context;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Infrastructure;

namespace Quillity.Application.User.NotepadPage.Commands;

public class DeleteNotepadPageCommand : IRequest<Unit>
{
    public required Guid PageId { get; set; }
}

public class DeleteNotepadCommandHandler : IRequestHandler<DeleteNotepadPageCommand, Unit>
{
    private readonly QuillityDbContext _dbContext;
    private readonly UserContext _userContext;

    public DeleteNotepadCommandHandler(QuillityDbContext dbContext, UserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<Unit> Handle(DeleteNotepadPageCommand request, CancellationToken cancellationToken)
    {
        var existingPage = await _dbContext.NotepadPages
            .FirstOrDefaultAsync(page =>
                StringHelper.IsEqualIgnoreCase(page.Id, request.PageId) &&
                StringHelper.IsEqualIgnoreCase(page.OwnerId, _userContext.User.Id), cancellationToken)
            ?? throw new QuillityException(
                StatusCodes.NOT_FOUND,
                ErrorMessages.GetNotFoundError(ErrorMessageModel.NotepadPage));

        _dbContext.NotepadPages.Remove(existingPage);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
