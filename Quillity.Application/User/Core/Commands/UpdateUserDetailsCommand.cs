using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Application.DTOs;
using Quillity.Common.Context;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Infrastructure;

namespace Quillity.Application.User.Core.Commands;

public class UpdateUserDetailsCommand : IRequest<UserDetailsDTO>
{

    public required string FullName { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public string? ProfilePictureUrl { get; set; }
}

public class UpdateUserDetailsCommandHandler : IRequestHandler<UpdateUserDetailsCommand, UserDetailsDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly UserContext _userContext;

    public UpdateUserDetailsCommandHandler(QuillityDbContext dbContext, UserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }

    public async Task<UserDetailsDTO> Handle(UpdateUserDetailsCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _dbContext.Users
            .FirstOrDefaultAsync(user => StringHelper.IsEqualIgnoreCase(user.Id, _userContext.User.Id), cancellationToken)
            ?? throw new QuillityException(StatusCodes.UNAUTHORIZED, "You are not authorized to update details for this user.");
        
        existingUser.FullName = request.FullName;
        existingUser.UserName = request.UserName;
        existingUser.Email = request.Email;
        existingUser.ProfilePictureUrl = StringHelper.NullIfWhiteSpace(request.ProfilePictureUrl);
        existingUser.InitialsForFallback = StringHelper.GetInitialLetters(existingUser.UserName);
        
        _dbContext.Users.Update(existingUser);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ObjectMapper.ToUserDetailsDTO(existingUser);
    }
}
