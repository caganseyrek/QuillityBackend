using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Auth.Protectors;
using Quillity.Common.Exceptions;
using Quillity.Common.Response;
using Quillity.Infrastructure;

namespace Quillity.Application.Auth;

public class EmailVerificationCommand : IRequest<Unit>
{
    public required string EmailVerificationToken { get; set; }
}

public class EmailVerificationCommandHandler : IRequestHandler<EmailVerificationCommand, Unit>
{
    private readonly QuillityDbContext _dbContext;
    private readonly EmailVerificationTokenProtector _protector;

    public EmailVerificationCommandHandler(QuillityDbContext dbContext, EmailVerificationTokenProtector protector)
    {
        _dbContext = dbContext;
        _protector = protector;
    }

    public async Task<Unit> Handle(EmailVerificationCommand request, CancellationToken cancellationToken)
    {
        var verificationToken = _protector.Unprotect(request.EmailVerificationToken);
        var tokenParts = verificationToken.Split(":");
        if (tokenParts.Length != 2)
            throw new QuillityException(StatusCodes.UNAUTHORIZED, "This verification link is invalid");

        var userEmailFromToken = tokenParts.ElementAt(0);
        var timestampFromToken = tokenParts.ElementAt(1);
        var userDetails = await _dbContext.Users
            .FirstOrDefaultAsync(user =>user.Email.Equals(userEmailFromToken), cancellationToken)
            ?? throw new QuillityException(StatusCodes.UNAUTHORIZED, "This verification link is invalid");

        if (userDetails.EmailVerified is true)
            throw new QuillityException(StatusCodes.BAD_REQUEST, "This verification link is invalid");

        // Account is not verified and does not have a token, seems like system error
        if (userDetails.EmailVerificationToken is null || userDetails.EmailVerificationTokenExpirationInMs is null)
            throw new QuillityException(StatusCodes.UNAUTHORIZED, "This verification link is invalid");

        if (!long.TryParse(timestampFromToken, out var timestampFromTokenAsLong))
            throw new QuillityException(StatusCodes.SERVER_ERROR, "This verification link is invalid");

        if (timestampFromTokenAsLong != userDetails.EmailVerificationTokenExpirationInMs
            || timestampFromTokenAsLong <= DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())
            throw new QuillityException(StatusCodes.UNAUTHORIZED, "This verification link is invalid");

        userDetails.EmailVerificationToken = null;
        userDetails.EmailVerificationTokenExpirationInMs = null;
        userDetails.EmailVerified = true;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
