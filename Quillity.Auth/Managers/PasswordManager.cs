using Microsoft.AspNetCore.Identity;
using Quillity.Common.Models;

namespace Quillity.Auth.Managers;

public class PasswordManager
{
    private readonly PasswordHasher<UserModel> _passwordHasher = new();

    public string HashRawPassword(UserModel user, string rawPassword)
    {
        return _passwordHasher.HashPassword(user, rawPassword);
    }

    public bool VerifyHashedPassword(UserModel user, string hashedPassword, string providedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
        return result == PasswordVerificationResult.Success;
    }
}
