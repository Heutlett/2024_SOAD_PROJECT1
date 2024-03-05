using System.Text.Json.Serialization;

namespace MyServiceApi.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SourceType
    {
        Local,
        AI,
        Dynamic
    }
}