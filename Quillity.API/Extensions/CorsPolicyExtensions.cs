namespace Quillity.API.Extensions;

public static class CorsPolicyExtensions
{
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
    {
        var origin = configuration["API:CORS:Origin"] ?? throw new Exception("Cors origin value is invalid");

        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", policy =>
            {
                policy.WithOrigins(origin).AllowCredentials().AllowAnyHeader().AllowAnyMethod();
            });
        });

        return services;
    }
}
