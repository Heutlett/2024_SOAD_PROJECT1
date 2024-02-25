using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyServiceApi.Models;

namespace MyServiceApi.Services.MealService
{
    public interface IMealService
    {
        ServiceResponse<List<Meal>> GenerateMealRecommendationNative(Meal meal);
        ServiceResponse<List<Meal>> GenerateMealRecommendationAI(Meal meal);
        ServiceResponse<List<Meal>> GenerateMealRecommendationExternal(Meal meal);

        Task<ServiceResponse<Meal>> CreateMeal(CreateMealDto newMeal);
        Task<ServiceResponse<List<Meal>>> ReadMeals();
    }
}