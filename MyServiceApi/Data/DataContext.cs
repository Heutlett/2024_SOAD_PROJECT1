using MyServiceApi.Models;
using Newtonsoft.Json;
using System;

namespace MyServiceApi.Data;

public class DataContext
{
    private MealRecommendation? allRecommendations;
    private string jsonFilePath = "./Data/mealsData.json";

    public DataContext()
    {
        string json = System.IO.File.ReadAllText(jsonFilePath);
        allRecommendations = JsonConvert.DeserializeObject<MealRecommendation>(json);
    }

    // Get the missing categories for recommendation
    public string GetMissingRecommendations(MealRequest mealRequest)
    {
        int quantityOfMeals = mealRequest.Meals.Count;
        Random random = new Random();
        
        try
        {

            if (allRecommendations == null)
            {
                throw new Exception("An error occurred while getting the recommendation.");
            }
            if(allRecommendations.Desserts== null || allRecommendations.Drinks == null || allRecommendations.MainCourses== null)
            {
                throw new Exception("An error occurred while getting the recommendation.");
            }

            List<Meal>? desserts = allRecommendations.Desserts;
            List<Meal>? mainCourses = allRecommendations.MainCourses;
            List<Meal>? drinks = allRecommendations.Drinks;

            int randomNumber = random.Next(0, 20);

            string recommendedMealsJson = "";

            if (quantityOfMeals == 1)
            {
                if (mealRequest.Meals[0].Course == CourseType.MainCourse)
                {
                    Meal recommendedDrink = drinks[randomNumber];
                    recommendedDrink.Course = CourseType.Drink; 
                    Meal recommendedDessert = desserts[randomNumber];
                    recommendedDessert.Course = CourseType.Dessert; 
                    
                    recommendedMealsJson = $"{{\"RecommendedMeals\": [{{\"Name\": \"{recommendedDrink.Name}\", \"CourseType\": \"{recommendedDrink.Course}\"}}, {{\"Name\": \"{recommendedDessert.Name}\", \"CourseType\": \"{recommendedDessert.Course}\"}}]}}";
                }
                else if (mealRequest.Meals[0].Course == CourseType.Dessert)
                {
                    Meal recommendedMainCourse = mainCourses[randomNumber];
                    recommendedMainCourse.Course = CourseType.MainCourse; 
                    Meal recommendedDrink = drinks[randomNumber];
                    recommendedDrink.Course = CourseType.Drink; 
                    
                    recommendedMealsJson = $"{{\"RecommendedMeals\": [{{\"Name\": \"{recommendedMainCourse.Name}\", \"CourseType\": \"{recommendedMainCourse.Course}\"}}, {{\"Name\": \"{recommendedDrink.Name}\", \"CourseType\": \"{recommendedDrink.Course}\"}}]}}";
                }
                else if (mealRequest.Meals[0].Course == CourseType.Drink)
                {
                    Meal recommendedMainCourse = mainCourses[randomNumber];
                    recommendedMainCourse.Course = CourseType.MainCourse; 
                    Meal recommendedDessert = desserts[randomNumber];
                    recommendedDessert.Course = CourseType.Dessert; 
                    
                    recommendedMealsJson = $"{{\"RecommendedMeals\": [{{\"Name\": \"{recommendedMainCourse.Name}\", \"CourseType\": \"{recommendedMainCourse.Course}\"}}, {{\"Name\": \"{recommendedDessert.Name}\", \"CourseType\": \"{recommendedDessert.Course}\"}}]}}";
                }
                else
                {
                    throw new Exception("An error occurred while getting the parameters of the request.");
                }
            }
            else if (quantityOfMeals == 2)
            {
                var missingType = GetMissingMealType(mealRequest);
                Meal recommendedMeal;
                
                if (missingType == CourseType.MainCourse)
                {
                    recommendedMeal = mainCourses[randomNumber];
                    recommendedMeal.Course = CourseType.MainCourse;
                }
                else if (missingType == CourseType.Dessert)
                {
                    recommendedMeal = desserts[randomNumber];
                    recommendedMeal.Course = CourseType.Dessert;
                }
                else if (missingType == CourseType.Drink)
                {
                    recommendedMeal = drinks[randomNumber];
                    recommendedMeal.Course = CourseType.Drink;
                }
                else
                {
                    throw new Exception("An error occurred while getting the parameters of the request.");
                }
                
                recommendedMealsJson = $"{{\"RecommendedMeals\": [{{\"Name\": \"{recommendedMeal.Name}\", \"CourseType\": \"{recommendedMeal.Course}\"}}]}}";
            }
            else
            {
                throw new Exception("An error occurred while getting the parameters of the request.");
            }

            return recommendedMealsJson;
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }




    // Helper method to get the missing meal type
    private CourseType GetMissingMealType(MealRequest mealRequest)
    {
        var mealTypes = Enum.GetValues(typeof(CourseType)).Cast<CourseType>().ToList();
        foreach (var mealType in mealTypes)
        {
            if (!mealRequest.Meals.Any(meal => meal.Course == mealType))
            {
                return mealType;
            }
        }
        throw new Exception("Invalid meal request.");
    }
}