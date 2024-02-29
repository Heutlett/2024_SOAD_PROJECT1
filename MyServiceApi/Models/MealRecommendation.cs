namespace MyServiceApi.Models
{
    public class MealRecommendation
    {
        public List<Meal>? Desserts { get; set; }
        public List<Meal>? MainCourses { get; set; }
        public List<Meal>? Drinks { get; set; }
    }
}