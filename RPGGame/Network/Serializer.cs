namespace RPGGame.Network;

using System.Text.Json;

public static class Serializer
{
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static string Serialize<T>(T obj)
    {
        return JsonSerializer.Serialize(obj, Options);
    }

    public static T Deserialize<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json, Options);
    }
}