using ProfileAPI;
using ProfileAPI.Configuration.WebAppHostStartup;
using ProfileAPI.Infrastructure.Database;
using ProfileAPI.Infrastructure.Testing;

var databaseContainer = new CosmosDbEmulatorContainer();

await databaseContainer.InitializeAsync();

var builder = WebApplication
    .CreateBuilder(args)
    .ConfigureHost();

var startup = new Startup(builder.Configuration, builder.Environment);

startup.ConfigureServices(builder.Services);

var provider = builder.Services.BuildServiceProvider();

await RunDatabaseMigrationsAsync(provider);
//await PostStartupAsync(provider);

var app = builder.Build();

startup.Configure(app);

app.MapControllers();

app.Run();

static async Task RunDatabaseMigrationsAsync(IServiceProvider serviceProvider)
{
    var scope = serviceProvider.CreateScope();
    var migrationRunner = scope.ServiceProvider.GetRequiredService<ProfileDbContext>();
    await migrationRunner.Database.EnsureCreatedAsync();
}

//static async Task PostStartupAsync(IServiceProvider serviceProvider)
//{
//    //var creationService = serviceProvider.GetRequiredService<IMessageEntityCreationService>();
//    //await creationService.CreateTopicAndSubscriptionIfNotExists<VehicleOnSaleSubscriptionCreatedEvent>();
//}

public partial class Program
{
    // this partial class is required for the integration tests to work with .net 6
}
