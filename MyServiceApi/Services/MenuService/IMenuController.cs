using MyServiceApi.Models;

namespace MyServiceApi.Services.MenuService
{
    public interface IMenuController
    {
        Task<ServerResponse<MealResponse>> GenerateMealRecommendation(MealRequest mealRequest);

        Task<ServerResponse<MealResponse?>> GenerateMealRecommendationAI(MealRequest mealRequest);

        Task<ServerResponse<MealResponse>> GenerateMealRecommendationLocal(MealRequest mealRequest);

        Task<ServerResponse<MealResponse>> GenerateMealRecommendationDynamic(MealRequest mealRequest);
    }
}