using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.DataProtection;
using Quillity.API.Extensions;
using Quillity.API.Middleware;
using Quillity.Application;
using Quillity.Application.Auth;
using Quillity.Application.Projects.Core.Commands;
using Quillity.Application.Workspaces.Core.Commands;
using Quillity.Application.Workspaces.Core.Validators;
using Quillity.Auth;
using Quillity.Infrastructure;
using Quillity.Logging;
using System.Text.Json;

namespace Quillity.API;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var keyStoragePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ASP.NET",
            "DataProtection-Keys");

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddCorsPolicy(builder.Configuration);
        builder.Services.AddDataProtection()
            .SetApplicationName("Quillity")
            .PersistKeysToFileSystem(new DirectoryInfo(keyStoragePath))
            .SetDefaultKeyLifetime(TimeSpan.FromDays(60));

        builder.Services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblyContaining<RegisterCommand>();
            config.RegisterServicesFromAssemblyContaining<CreateWorkspaceCommand>();
            config.RegisterServicesFromAssemblyContaining<CreateProjectCommand>();
        });
        //builder.Services.AddValidatorsFromAssemblyContaining<RegisterCommandValidator>();
        builder.Services.AddValidatorsFromAssemblyContaining<CreateWorkspaceCommandValidator>();
        //builder.Services.AddValidatorsFromAssemblyContaining<CreateProjectCommandValidator>();

        builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        builder.Services.AddLoggingModule();
        builder.Services.AddAuthModule(builder.Configuration);
        builder.Services.AddInfrastructureModule(builder.Configuration);

        builder.Services
            .AddControllers(options => options.Conventions.Add(new PrefixConvention(builder.Configuration)))
            .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();

        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        app.UseMiddleware<UserContextMiddleware>();

        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}