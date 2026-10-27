using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quillity.Infrastructure.Interceptors;

namespace Quillity.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructureModule(this IServiceCollection services, IConfiguration configuration)
    {
        // We use default exception here because non-custom exceptions returns internal server error message to the client.
        // This is done in various places in the codebase
        var connectionString = configuration.GetConnectionString("MainDatabase")
            ?? throw new Exception("Connection string value is invalid");

        services.AddScoped<EntityAuditInterceptor>();
        services.AddDbContext<QuillityDbContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(connectionString);
            options.AddInterceptors(serviceProvider.GetRequiredService<EntityAuditInterceptor>());
        });

        services.AddScoped<EmailSender>();

        return services;
    }
}
