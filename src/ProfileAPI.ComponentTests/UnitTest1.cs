using ProfileAPI.Infrastructure.Testing;
using Xunit;

namespace ProfileAPI.ComponentTests;

[Collection(nameof(IntegrationTestCollection))]
public class UnitTest1
{
#pragma warning disable IDE0052 // Remove unread private members
    private readonly IntegrationTestCollectionFixture _integrationTestCollectionFixture;
#pragma warning restore IDE0052 // Remove unread private members

    public UnitTest1(IntegrationTestCollectionFixture integrationTestCollectionFixture)
        => _integrationTestCollectionFixture = integrationTestCollectionFixture;

    [Fact]
    public async Task Test1()
    {
        await Task.Delay(2);
        await Task.Delay(2);
    }
}

[CollectionDefinition(nameof(IntegrationTestCollection))]
public class IntegrationTestCollection : ICollectionFixture<IntegrationTestCollectionFixture>
{
}

public sealed class IntegrationTestCollectionFixture : IAsyncLifetime
{
    public readonly CosmosDbEmulatorContainer _container;

    public IntegrationTestCollectionFixture()
        => _container = new CosmosDbEmulatorContainer();

    public async Task InitializeAsync()
        => await _container.InitializeAsync();

    public Task DisposeAsync()
        => _container.DisposeAsync().AsTask();
}
