using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Quillity.Common.Response;

namespace Quillity.API.Filters;

public class ResponseWrapperFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult objectResult &&
            objectResult.Value is not ResponseObject<object> &&
            objectResult.Value is not ResponseObject<object[]>)
        {
            var wrappedResponse = ResponseObjectFactory.Success(data: objectResult.Value);

            context.Result = new ObjectResult(wrappedResponse)
            {
                StatusCode = objectResult.StatusCode ?? 200,
            };
        }
        await next();
    }
}
