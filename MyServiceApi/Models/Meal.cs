using System.Text.Json.Serialization;

namespace MyServiceApi.Models
{
    public class Meal
    {
        [JsonPropertyName("Name")]
        public string? Name { get; set; }

        [JsonPropertyName("CourseType")]
        public CourseType? Course { get; set; }
    }
}