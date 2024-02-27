using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyServiceApi.Models;

namespace MyServiceApi.Services.MenuService
{
    public interface IMenuController
    {
        Task<ServerResponse<MealResponse>> GenerateMealRecommendation(MealRequest mealRequest);

        Task<ServerResponse<MealResponse?>> GenerateMealRecommendationAI(MealRequest mealRequest);

        Task<MealResponse> GenerateMealRecommendationExternal(MealRequest mealRequest);

        Task<MealResponse> GenerateMealRecommendationNative(MealRequest mealRequest);
    }
}