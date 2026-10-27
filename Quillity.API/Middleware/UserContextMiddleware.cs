using Microsoft.EntityFrameworkCore;
using Quillity.Common.Context;
using Quillity.Infrastructure;
using System.Security.Claims;

namespace Quillity.API.Middleware;

public class UserContextMiddleware
{
    private readonly RequestDelegate _next;

    public UserContextMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext, UserContext userContext, QuillityDbContext dbContext)
    {
        var principal = httpContext.User;
        if (principal.Identity?.IsAuthenticated != true)
        {
            await _next(httpContext);
            return;
        }

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
        var userEmailClaim = principal.FindFirst(ClaimTypes.Email);
        if (userIdClaim is null || userEmailClaim is null || !Guid.TryParse(userIdClaim.Value, out var parsedUserId))
        {
            await _next(httpContext);
            return;
        }

        var userDetails = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id.Equals(parsedUserId) && user.Email.Equals(userEmailClaim));
        if (userDetails is null)
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        userContext.User = userDetails;
        await _next(httpContext);
    }
}
