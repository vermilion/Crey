using System.Text.Json;

namespace Crey.Helper;

public static class JsonHelper
{
    public static string ToJson<T>(T data)
    {
        return JsonSerializer.Serialize(data);
    }

    public static T FromJson<T>(string json)
    {
        return JsonSerializer.Deserialize<T>(json);
    }

    public static object FromJson(string content, Type type)
    {
        return JsonSerializer.Deserialize(content, type);
    }
}
