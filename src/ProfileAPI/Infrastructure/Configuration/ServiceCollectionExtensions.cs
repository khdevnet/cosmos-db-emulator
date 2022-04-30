using Microsoft.OpenApi.Models;

namespace ProfileAPI.Infrastructure.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            var sourceBranch = Environment.GetEnvironmentVariable("Build.SourceBranchName");
            var commitId = Environment.GetEnvironmentVariable("Build.SourceVersion");

            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "ProfileAPI",
                Version = sourceBranch is not null && commitId is not null ? $"Branch - {sourceBranch} (Commit - {commitId})" : "v1",
                Description = $"Started at {DateTime.UtcNow} UTC.",
            });

            options.CustomSchemaIds(type => type.ToString());
        });

        return services;
    }

    public static IServiceCollection AddCORS(this IServiceCollection services)
    {
        return services.AddCors(options => options
             .AddDefaultPolicy(builder => builder
                 .AllowCredentials()
                 .SetIsOriginAllowed(o => true)
                 .AllowAnyMethod()
                 .AllowAnyHeader()));
    }
}
