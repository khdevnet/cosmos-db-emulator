using System.Diagnostics;
using System.Net;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;

namespace ProfileAPI.Infrastructure.Testing;

public sealed class CosmosDbEmulatorContainer : IAsyncDisposable
{
    private readonly IOutputConsumer _consumer = Consume.RedirectStdoutAndStderrToStream(new MemoryStream(), new MemoryStream());

    public readonly ITestcontainersContainer _container;

    public CosmosDbEmulatorContainer()
    {
        _container = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator")
          .WithName("azure-cosmos-emulator")
          .WithExposedPort(8081)
          .WithExposedPort(10251)
          .WithExposedPort(10252)
          .WithExposedPort(10253)
          .WithExposedPort(10254)
          .WithPortBinding(8081)
          .WithPortBinding(10251, true)
          .WithPortBinding(10252, true)
          .WithPortBinding(10253, true)
          .WithPortBinding(10254, true)
          .WithEnvironment("AZURE_COSMOS_EMULATOR_PARTITION_COUNT", "30")
          .WithEnvironment("AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE", "127.0.0.1")
          .WithEnvironment("AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE", "false")
          .WithOutputConsumer(_consumer)
          .WithWaitStrategy(Wait.ForUnixContainer()
            .UntilMessageIsLogged(_consumer.Stdout, "Started"))
          .Build();
    }

    public bool IsReady { get; private set; }

    public bool IsStarted { get; private set; }

    public async Task<string?> GetConnectionStringAsync()
    {
        await InitializeAsync();

        return GetConnectionString();
    }

    public async Task InitializeAsync()
    {
        if (IsStarted)
        {
            return;
        }

        await _container.StartAsync();

        IsStarted = true;

        await WaitStartupAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _container.DisposeAsync();
        _consumer.Dispose();
    }

    private async Task WaitStartupAsync(int attempts = 45, int delayBetweenAttemptsInSeconds = 2)
    {
        for (int i = 0; i < attempts; i++)
        {
            await Task.Delay(TimeSpan.FromSeconds(delayBetweenAttemptsInSeconds)).ConfigureAwait(false);

            if (await IsReadyAsync())
            {
                IsReady = true;
                return;
            }
        }
    }

    private async Task<bool> IsReadyAsync()
    {
        using (var handler = new HttpClientHandler())
        {
            handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;

            using (var client = new HttpClient(handler))
            {
                var response = await client.GetAsync($"{GetConnectionString()}/_explorer/emulator.pem")
                  .ConfigureAwait(false);

                var pem = await response.Content.ReadAsStringAsync()
                  .ConfigureAwait(false);

                Debug.WriteLine(pem);

                return response.StatusCode == HttpStatusCode.OK;
            }
        }
    }

    private string GetConnectionString()
    {
        var mappedPort = _container.GetMappedPublicPort(8081);
        var connectionString = $"https://localhost:{mappedPort}";
        return connectionString;
    }
}
