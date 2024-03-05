namespace MyServiceApi.Models
{
    public class MealRequest
    {
        public List<Meal> Meals { get; set; } = new List<Meal>();
        public SourceType SourceType { get; set; }
    }
}