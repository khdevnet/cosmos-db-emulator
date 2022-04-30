namespace ProfileAPI.Configuration.WebAppHostStartup;

public static class HostStartupBuilderExtensions
{
    public static WebApplicationBuilder ConfigureHost(this WebApplicationBuilder webAppBuilder)
    {
        var hostStartup =
            TryGetEnvHostStartupType(webAppBuilder.Environment.EnvironmentName, out var hostStartupType)
            ? CreateEnvHostStartup(hostStartupType, webAppBuilder.Host)
            : new HostStartup(webAppBuilder.Host);

        hostStartup.Configure();

        return webAppBuilder;
    }

    private static HostStartupBase CreateEnvHostStartup(Type hostStartupType, ConfigureHostBuilder hostBuilder)
        => (HostStartupBase)Activator.CreateInstance(hostStartupType, hostBuilder);

    private static bool TryGetEnvHostStartupType(string environmentName, out Type hostStartupType)
    {
        var envHostStartupType =
            AppDomain.CurrentDomain
                         .GetAssemblies()
                         .SelectMany(a => a.GetTypes())
                         .Where(t => t.BaseType == typeof(HostStartupBase))
                         .Where(t => !string.IsNullOrEmpty(t.Name))
                         .FirstOrDefault(t => t.Name.StartsWith(environmentName, StringComparison.InvariantCultureIgnoreCase));

        hostStartupType = envHostStartupType!;

        return envHostStartupType is not null;
    }
}
