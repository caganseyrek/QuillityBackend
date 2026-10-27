using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Quillity.Auth.Managers;
using Quillity.Auth.Protectors;
using Quillity.Common;
using Quillity.Common.Exceptions;
using Quillity.Common.Helpers;
using Quillity.Common.Models;
using Quillity.Common.Response;
using Quillity.Common.Values;
using Quillity.Infrastructure;

namespace Quillity.Application.Auth;

public class RegisterCommand : IRequest<Unit>
{
    public required string FullName { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string? ProfilePictureUrl { get; set; }
}

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Unit>
{
    private readonly QuillityDbContext _dbContext;
    private readonly PasswordManager _passwordManager;
    private readonly EmailVerificationTokenProtector _protector;
    private readonly EmailSender _emailSender;

    public RegisterCommandHandler(
        QuillityDbContext dbContext,
        EmailVerificationTokenProtector protector,
        EmailSender emailSender,
        PasswordManager passwordManager)
    {
        _dbContext = dbContext;
        _protector = protector;
        _emailSender = emailSender;
        _passwordManager = passwordManager;
    }

    public async Task<Unit> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existingEmail = await _dbContext.Users
            .Where(user => StringHelper.IsEqualIgnoreCase(user.Email, request.Email))
            .AnyAsync(cancellationToken);
        if (existingEmail)
            throw new QuillityException(StatusCodes.BAD_REQUEST, "This username or email is not available");

        var existingUserName = await _dbContext.Users
            .Where(user => StringHelper.IsEqualIgnoreCase(user.UserName, request.UserName))
            .AnyAsync(cancellationToken);
        if (existingUserName)
            throw new QuillityException(StatusCodes.BAD_REQUEST, "This username or email is not available");

        var newUser = new UserModel
        {
            FullName = request.FullName,
            UserName = request.UserName,
            Email = request.Email,
            ProfilePictureUrl = request.ProfilePictureUrl ?? DefaultValues.Images.DEFAULT_USER_PROFILE_PICTURE_URL,
            InitialsForFallback = StringHelper.GetInitialLetters(request.FullName),
        };
        newUser.PasswordHash = _passwordManager.HashRawPassword(newUser, request.Password);

        var userPreferences = new UserPreferencesModel
        {
            UserId = newUser.Id,
            User = newUser,
            PreferredUiTheme = DefaultValues.UserPreferences.DEFAULT_PREFERRED_UI_THEME,
            UserTimeZone = DefaultValues.UserPreferences.DEFAULT_USER_TIME_ZONE,
            FirstDayOfWeek = DefaultValues.UserPreferences.DEFAULT_FIRST_DAY_OF_WEEK,
            DateFormat = DefaultValues.UserPreferences.DEFAULT_DATE_FORMAT,
            TimeFormat = DefaultValues.UserPreferences.DEFAULT_TIME_FORMAT,
        };

        var verificationTokenExpirationTime = DateTimeOffset.UtcNow.AddHours(1).ToUnixTimeMilliseconds();
        var verificationTokenPayload = $"{newUser.Email}:{verificationTokenExpirationTime}";
        var verificationToken = _protector.Protect(verificationTokenPayload);
        newUser.EmailVerificationToken = verificationToken;
        newUser.EmailVerificationTokenExpirationInMs = verificationTokenExpirationTime;

        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var verificationMail = new EmailMessage
            {
                To = newUser.Email,
                Subject = "Quillity Email Verification",
                Body = $"<p>Hi {newUser.FullName},</p> " +
                    $"<p>Thank you for registering with Quillity! To complete your registration, please verify your email address by clicking the link below:</p>" +
                    $"<p><a href=\"https://app.quillity.com/verify-email?token={verificationToken}\">Verify Email</a></p>" +
                    "<p>If you did not request this verification, please ignore this email.</p>" +
                    "<p>Best regards,<br/>The Quillity Team</p>",
            };
            await _emailSender.SendAsync(verificationMail, cancellationToken);

            await _dbContext.Users.AddAsync(newUser, cancellationToken);
            await _dbContext.UserPreferences.AddAsync(userPreferences, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new QuillityException(StatusCodes.SERVER_ERROR, ErrorMessages.GetServerError());
        }
        return Unit.Value;
    }
}
