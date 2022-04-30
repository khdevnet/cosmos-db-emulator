namespace ProfileAPI.Configuration.Options;

public class AzureDisabledFeaturesOptions
{
    public const string SectionKey = "Azure";

    public List<string> DisabledFeatures { get; set; } = new();
}
