using System.Text.Json;
namespace Afterburner.Utils;

public static class JsonOptions
{
    public static JsonSerializerOptions Default()
    {
        return new JsonSerializerOptions()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }
}
