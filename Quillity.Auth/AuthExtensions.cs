using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Quillity.Auth.Managers;
using Quillity.Auth.Protectors;
using Quillity.Common.Context;
using System.Text;

namespace Quillity.Auth;

public static class AuthExtensions
{
    public static IServiceCollection AddAuthModule(this IServiceCollection services, IConfiguration configuration)
    {
        var authConfig = new AuthConfig(configuration);

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = authConfig.Issuer,
                    ValidAudience = authConfig.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authConfig.AccessTokenKey)),
                    ClockSkew = TimeSpan.FromMinutes(1),
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Cookies["atq"];
                        if (!string.IsNullOrEmpty(token))
                            context.Token = token;

                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();

        services.AddScoped<UserContext>();

        services.AddScoped<TokenManager>();
        services.AddScoped<PasswordManager>();

        services.AddSingleton<EmailVerificationTokenProtector>();
        services.AddSingleton<PasswordResetTokenProtector>();

        services.AddScoped<PermissionService>();

        return services;
    }
}

public class AuthConfig
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string AccessTokenKey { get; set; }

    public AuthConfig(IConfiguration configuration)
    {
        Issuer = configuration["JWT:Issuer"] ?? throw new Exception("JWT issuer value is invalid");
        Audience = configuration["JWT:Audience"] ?? throw new Exception("JWT audience value is invalid");
        AccessTokenKey = configuration["JWT:AccessTokenKey"] ?? throw new Exception("JWT access token key value is invalid");
    }
}