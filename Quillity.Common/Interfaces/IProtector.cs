namespace Quillity.Common.Interfaces;

public interface IProtector
{
    string Protect(string input);
    string Unprotect(string input);
}
