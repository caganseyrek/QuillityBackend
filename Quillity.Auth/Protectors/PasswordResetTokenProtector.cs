using Microsoft.AspNetCore.DataProtection;
using Quillity.Common.Interfaces;

namespace Quillity.Auth.Protectors;

public class PasswordResetTokenProtector : IProtector
{
    private readonly IDataProtector _protector;

    public PasswordResetTokenProtector(IDataProtectionProvider protectionProvider)
    {
        _protector = protectionProvider.CreateProtector("Quillity.PasswordReset");
    }

    public string Protect(string input) => _protector.Protect(input);
    public string Unprotect(string input) => _protector.Unprotect(input);
}
