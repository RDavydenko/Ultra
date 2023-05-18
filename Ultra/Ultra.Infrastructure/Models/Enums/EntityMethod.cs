using System.Text.Json.Serialization;

namespace Ultra.Infrastructure.Models.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EntityMethod
    {
        Read,
        Create,
        Update,
        Delete,
    }
}
