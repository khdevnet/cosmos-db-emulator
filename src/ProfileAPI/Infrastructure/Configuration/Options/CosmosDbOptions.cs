namespace ProfileAPI.Infrastructure.Configuration.Options;

public class CosmosDbOptions
{
    public const string SectionKey = "CosmosDb";

    public string AccountEndpoint { get; set; } = string.Empty;

    public string AccountKey { get; set; } = string.Empty;
}
