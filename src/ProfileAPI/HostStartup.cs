using ProfileAPI.Configuration.WebAppHostStartup;

namespace ProfileAPI;

public class HostStartup : HostStartupBase
{
    public HostStartup(ConfigureHostBuilder hostBuilder)
        => Host = hostBuilder;

    public ConfigureHostBuilder Host { get; }

    public override void Configure()
    {
        Host.ConfigureAppConfiguration((hostBuilderContext, configBuilder)
            => configBuilder
                     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .AddJsonFile($"appsettings.{hostBuilderContext.HostingEnvironment.EnvironmentName}.json", optional: true)
                     .AddEnvironmentVariables());
    }
}
