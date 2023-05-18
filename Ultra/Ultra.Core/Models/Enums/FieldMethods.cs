using System.Text.Json.Serialization;

namespace Ultra.Core.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum FieldMethods
    {
        Created,
        Updated,
        Patched,
    }
}
