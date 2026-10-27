namespace Quillity.Common.Response;

public static class ResponseObjectFactory
{
    public static ResponseObject<T> Success<T>(T data, string message = "") =>
        new()
        {
            Success = true,
            Message = message,
            Data = data,
            Errors = null,
        };

    public static ResponseObject<object> Error(List<ResponseError> errors, string message) =>
        new()
        {
            Success = false,
            Message = message,
            Data = null,
            Errors = errors,
        };
}

public class ResponseObject<TResponseData>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public TResponseData? Data { get; set; }
    public List<ResponseError>? Errors { get; set; }
}

public class ResponseError
{
    public string ErrorMessage { get; set; } = string.Empty;
}
