//using System.Net.Mime;
//using Ardalis.GuardClauses;
//using ProfileAPI.ApplicationCore.Common.Domain;
//using ProfileAPI.Infrastructure.Configuration.Options;
//using Microsoft.Azure.Cosmos;
//using Microsoft.Extensions.Options;

//namespace ProfileAPI.Infrastructure.HostedServices;

//public class SendEventsHostedService : IHostedService
//{
//    private readonly CosmosDbOptions _cosmosDbOptions;
//    private readonly ServiceBusClient _serviceBusClient;
//    private readonly ICorrelationIdFactory _correlationIdFactory;
//    private readonly ILogger<SendEventsHostedService> _logger;

//    public SendEventsHostedService(
//        IOptions<CosmosDbOptions> cosmosDbOptions,
//        ServiceBusClient serviceBusClient,
//        ICorrelationIdFactory correlationIdFactory,
//        ILogger<SendEventsHostedService> logger)
//    {
//        _cosmosDbOptions = Guard.Against.Null(cosmosDbOptions.Value);
//        _serviceBusClient = serviceBusClient;
//        _correlationIdFactory = correlationIdFactory;
//        _logger = logger;
//    }

//    public async Task StartAsync(CancellationToken cancellationToken)
//    {
//        CosmosClient client = new CosmosClient(_cosmosDbOptions.AccountEndpoint, _cosmosDbOptions.AccountKey);

//        var db = client.GetDatabase($"ProfileAPITRDevelopmentDB");

//        var eventsLogContainer = db.GetContainer("EventLogs");

//        ContainerProperties leaseContainerProperties = new ContainerProperties("sendEventsLeases", "/id");
//        Container leaseContainer = await db.CreateContainerIfNotExistsAsync(leaseContainerProperties, cancellationToken: cancellationToken);

//        var builder = eventsLogContainer.GetChangeFeedProcessorBuilder<EventLog>("migrationProcessor",
//           async (ChangeFeedProcessorContext context, IReadOnlyCollection<EventLog> eventLogs, CancellationToken cancellationToken)
//           =>
//           {
//               _logger.LogInformation(eventLogs.Count + " Changes Received");

//               foreach (var eventLog in eventLogs)
//               {
//                   var sender = _serviceBusClient.CreateSender(eventLog.TopicName);
//                   await sender.SendMessageAsync(CreateServiceBusMessageWithJsonBodyFrom(eventLog.Data, _correlationIdFactory.CorrelationId), cancellationToken);
//               }
//           });

//        var processor = builder
//                        .WithInstanceName("changeFeedConsole")
//                        .WithLeaseContainer(leaseContainer)
//                        .Build();

//        await processor.StartAsync();
//    }

//    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

//    private static ServiceBusMessage CreateServiceBusMessageWithJsonBodyFrom(string messageJson, string correlationId)
//    {
//        var serviceBusMessage = new ServiceBusMessage(new BinaryData(messageJson))
//        {
//            ContentType = MediaTypeNames.Application.Json,
//            CorrelationId = correlationId,
//        };

//        return serviceBusMessage;
//    }
//}
