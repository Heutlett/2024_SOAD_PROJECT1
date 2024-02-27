using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyServiceApi.Models
{
    public class MealResponse
    {
        public List<Meal>? RecommendedMeals { get; set; }
    }
}