using System.Text.Json;

namespace ProfileAPI.ApplicationCore.Common;

public static class JsonSerializerExtensions
{
    public static string ToJson(this object data)
        => JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });
}
