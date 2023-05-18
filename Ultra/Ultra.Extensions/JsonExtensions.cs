using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ultra.Extensions
{
    public static class JsonExtensions
    {
        public readonly static JsonSerializerOptions Default;

        static JsonExtensions()
        {
            Default = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            Default.Converters.Add(new JsonStringEnumConverter());
        }

        public static T? FromJson<T>(this string json, JsonSerializerOptions? options = null) =>
            JsonSerializer.Deserialize<T>(json, options ?? Default);

        public static string ToJson<T>(this T obj, JsonSerializerOptions? options = null) => 
            JsonSerializer.Serialize(obj, options ?? Default);
    }
}
