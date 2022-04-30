using System.Text.Json;
using System.Text.Json.Serialization;
using ProfileAPI.Api.Filters;
using ProfileAPI.ApplicationCore.Common.Domain;
using ProfileAPI.ApplicationCore.Domain.Subscriptions;
using ProfileAPI.Infrastructure.Configuration.Options;
using ProfileAPI.Infrastructure.Database;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using ProfileAPI.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace ProfileAPI;

public class Startup
{
    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        Configuration = configuration;
        Environment = environment;
    }

    public IConfiguration Configuration { get; }

    public IWebHostEnvironment Environment { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddHealthChecks();

        services
            .AddEndpointsApiExplorer()
            .AddRouting(options => options.LowercaseUrls = true)
            .AddResponseCaching()
            .AddControllers(options =>
            {
                options.Filters.Add<ModelStateValidationActionFilterAttribute>();
                options.Filters.Add<DomainExceptionExceptionFilter>();
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });

        services
            .AddSwagger()
            .AddCORS();
        services.AddOptions<CosmosDbOptions>()
                 .Bind(Configuration.GetSection(CosmosDbOptions.SectionKey))
                 .ValidateDataAnnotations();

        services.AddDbContext<NotifyMeDbContext>(ConfigureDbCotextOptions);
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<ISubscriptionService, SubscriptionService>();

        //services.AddSingleton(sp =>
        //{
        //    var cosmosClientBuilder = new CosmosClientBuilder("");
        //    cosmosClientBuilder.WithHttpClientFactory(() =>
        //    {
        //        HttpMessageHandler httpMessageHandler = new HttpClientHandler()
        //        {
        //            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
        //        };

        //        return new HttpClient(httpMessageHandler);
        //    });
        //    cosmosClientBuilder.WithConnectionModeGateway();

        //    return cosmosClientBuilder.Build();
        //});

        //services.AddHostedService<SendEventsHostedService>();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseHttpsRedirection();

        if (Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler(app => app.Run(context =>
            {
                const string ErrorMessage = "Internal server error has occurred.";
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature.Error;

                return context.Response.WriteAsJsonAsync(new { Error = ErrorMessage });
            }));
        }

        app
            .UseRouting()
            .UseResponseCaching()
            .UseEndpoints(endpoints =>
                {
                    endpoints.MapHealthChecks("/health");
                    endpoints.MapControllers();
                });
    }

    private void ConfigureDbCotextOptions(IServiceProvider sp, DbContextOptionsBuilder options)
    {
        var cosmosdbOptions = sp.GetRequiredService<IOptions<CosmosDbOptions>>();
        var cosmosDbOptions = cosmosdbOptions.Value;
        options.UseCosmos(
            cosmosDbOptions.AccountEndpoint,
            cosmosDbOptions.AccountKey,
            GetDatabaseName(sp),
            contextOptions =>
            {
                contextOptions.HttpClientFactory(()
                       => new HttpClient(new HttpClientHandler
                       {
                           ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
                       })
                   );
            });
    }

    private string GetDatabaseName(IServiceProvider sp)
    {
        var hostEnv = sp.GetRequiredService<IWebHostEnvironment>();
        return $"ProfileAPI{hostEnv.EnvironmentName}DB";
    }
}
