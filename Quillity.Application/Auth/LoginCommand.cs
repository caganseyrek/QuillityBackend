using MediatR;
using Microsoft.EntityFrameworkCore;
using Quillity.Auth.Managers;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Response;
using Quillity.Infrastructure;

namespace Quillity.Application.Auth;

public class LoginDTO
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}

public class LoginCommand : IRequest<LoginDTO>
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginDTO>
{
    private readonly QuillityDbContext _dbContext;
    private readonly TokenManager _tokenManager;
    private readonly PasswordManager _passwordManager;

    public LoginCommandHandler(
        QuillityDbContext dbContext,
        TokenManager tokenManager,
        PasswordManager passwordManager)
    {
        _dbContext = dbContext;
        _tokenManager = tokenManager;
        _passwordManager = passwordManager;
    }

    public async Task<LoginDTO> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _dbContext.Users
            .FirstOrDefaultAsync(user => StringHelper.IsEqualIgnoreCase(user.Email, request.Email), cancellationToken)
            ?? throw new QuillityException(StatusCodes.UNAUTHORIZED, "Your email or password is wrong.");

        if (existingUser.IsSuspended)
            throw new QuillityException(StatusCodes.UNAUTHORIZED, "Your account is suspended. Please contact support.");

        var isPasswordValid = _passwordManager.VerifyHashedPassword(existingUser, existingUser.PasswordHash, request.Password);
        if (isPasswordValid is false)
            throw new QuillityException(StatusCodes.UNAUTHORIZED, "Your email or password is wrong.");

        if (existingUser.EmailVerificationToken is not null && existingUser.EmailVerified is false)
            throw new QuillityException(StatusCodes.UNAUTHORIZED, "You need to verify your email first.");

        var accessToken = _tokenManager.GenerateAccessToken(existingUser.Id.ToString(), existingUser.Email);
        var refreshToken = _tokenManager.GenerateRefreshToken(existingUser.Id.ToString(), existingUser.Email);
        return new LoginDTO
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
        };
    }
}
