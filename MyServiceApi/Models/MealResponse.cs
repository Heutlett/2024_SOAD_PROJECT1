using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace MyServiceApi.Models
{
    public class MealResponse
    {
        [JsonPropertyName("RecommendedMeals")]
        public List<Meal>? RecommendedMeals { get; set; }
    }
}