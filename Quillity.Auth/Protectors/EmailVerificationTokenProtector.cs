using Microsoft.AspNetCore.DataProtection;
using Quillity.Common.Interfaces;

namespace Quillity.Auth.Protectors;

public class EmailVerificationTokenProtector : IProtector
{
    private readonly IDataProtector _protector;

    public EmailVerificationTokenProtector(IDataProtectionProvider protectionProvider)
    {
        _protector = protectionProvider.CreateProtector("Quillity.EmailVerification");
    }

    public string Protect(string input) => _protector.Protect(input);
    public string Unprotect(string input) => _protector.Unprotect(input);
}
