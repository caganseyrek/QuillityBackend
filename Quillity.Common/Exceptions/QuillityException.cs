namespace Quillity.Common.Exceptions;

public class QuillityException : Exception
{
    public int StatusCode { get; set; }

    public QuillityException(int statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}
