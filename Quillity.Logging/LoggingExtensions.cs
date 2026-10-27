using Microsoft.Extensions.DependencyInjection;

namespace Quillity.Logging;

public static class LoggingExtensions
{
    public static IServiceCollection AddLoggingModule(this IServiceCollection services)
    {
        services.AddScoped<ActivityLogger>();
        return services;
    }
}
