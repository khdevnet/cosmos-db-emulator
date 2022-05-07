using Microsoft.Azure.Cosmos;
using ProfileAPI.Infrastructure.Testing;
using Xunit;

namespace ProfileAPI.ComponentTests;

[Collection(nameof(IntegrationTestCollection))]
public class UnitTest1
{
    private const string AccountKey = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

#pragma warning disable IDE0052 // Remove unread private members
    private readonly IntegrationTestCollectionFixture _integrationTestCollectionFixture;
#pragma warning restore IDE0052 // Remove unread private members

    public UnitTest1(IntegrationTestCollectionFixture integrationTestCollectionFixture)
        => _integrationTestCollectionFixture = integrationTestCollectionFixture;

    [Fact]
    public async Task Test1()
    {
        var host = _integrationTestCollectionFixture.Container.GetHost();

        var client = new CosmosClient(host, AccountKey, GetCosmosClientOptions());

        var database = await client.CreateDatabaseIfNotExistsAsync("mytest");
        var container = await database.Database.CreateContainerIfNotExistsAsync(new ContainerProperties()
        {
            Id = "cars",
            PartitionKeyPath = "/id",
        });

        await container.Container.CreateItemAsync(new { id = Guid.NewGuid(), make = "fiat" });
    }

    private static CosmosClientOptions GetCosmosClientOptions()
    {
        return new CosmosClientOptions()
        {
            HttpClientFactory = () =>
            {
                HttpMessageHandler httpMessageHandler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                };

                return new HttpClient(httpMessageHandler);
            },
            ConnectionMode = ConnectionMode.Gateway,
        };
    }
}

[CollectionDefinition(nameof(IntegrationTestCollection))]
public class IntegrationTestCollection : ICollectionFixture<IntegrationTestCollectionFixture>
{
}

public sealed class IntegrationTestCollectionFixture : IAsyncLifetime
{
    public readonly CosmosDbEmulatorContainer Container;

    public IntegrationTestCollectionFixture()
        => Container = new CosmosDbEmulatorContainer();

    public async Task InitializeAsync()
        => await Container.InitializeAsync();

    public Task DisposeAsync()
        => Container.DisposeAsync().AsTask();
}
