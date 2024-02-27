using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyServiceApi.Data;
using MyServiceApi.Models;

namespace MyServiceApi.Services.MenuService
{
    public class MenuController : IMenuController
    {
        private readonly DataContext _context;
        public MenuController(DataContext context)
        {
            _context = context;
        }

        public async Task<ServerResponse<MealResponse>> GenerateMealRecommendation(MealRequest mealRequest)
        {
            ServerResponse<MealResponse> serverResponse = new();

            try
            {
                switch (mealRequest.SourceType)
                {
                    case SourceType.Local: // Parte de PERSONA A
                        serverResponse.Success = true;
                        serverResponse.Message = "This is a response from local";
                        break;

                    case SourceType.AI: // Parte de PERSONA B
                        ServerResponse<MealResponse?> mealResponse = await GenerateMealRecommendationAI(mealRequest);

                        if (!mealResponse.Success)
                        {
                            throw new Exception(mealResponse.Message);
                        }

                        serverResponse.Data = mealResponse.Data;
                        serverResponse.Message = "This is a response from AI";
                        break;

                    case SourceType.Dynamic: // Parte de PERSONA C
                        serverResponse.Message = "This is a response from dynamic";
                        break;
                }
                serverResponse.Success = true;
            }
            catch (Exception e)
            {
                serverResponse.Message = e.Message;
                serverResponse.Success = false;
            }
            return serverResponse;
        }

        public async Task<ServerResponse<MealResponse?>> GenerateMealRecommendationAI(MealRequest mealRequest)
        {
            ServerResponse<MealResponse?> serverResponse = new();

            try
            {
                if (mealRequest.Meal == null)
                {
                    throw new Exception("The Meal property of the request is empty");
                }

                switch (mealRequest.Meal.Course)
                {
                    case CourseType.MainCourse:

                        serverResponse.Data = new MealResponse
                        {
                            RecommendedMeals =
                                [
                                    new Meal { Name = "Coca Cola", Course = CourseType.Drink },
                                    new Meal { Name = "Ice cream", Course = CourseType.Dessert }
                                ]
                        };

                        break;

                    case CourseType.Drink:

                        serverResponse.Data = new MealResponse
                        {
                            RecommendedMeals =
                                [
                                    new Meal { Name = "Pizza", Course = CourseType.MainCourse },
                                    new Meal { Name = "Ice cream", Course = CourseType.Dessert }
                                ]
                        };

                        break;

                    case CourseType.Dessert:

                        serverResponse.Data = new MealResponse
                        {
                            RecommendedMeals =
                                [
                                    new Meal { Name = "Pizza", Course = CourseType.MainCourse },
                                    new Meal { Name = "Coca Cola", Course = CourseType.Drink }
                                ]
                        };

                        break;
                }
            }
            catch (Exception e)
            {
                serverResponse.Success = false;
                serverResponse.Message = e.Message;
            }
            return serverResponse;
        }

        public Task<MealResponse> GenerateMealRecommendationExternal(MealRequest mealRequest)
        {
            throw new NotImplementedException();
        }

        public Task<MealResponse> GenerateMealRecommendationNative(MealRequest mealRequest)
        {
            throw new NotImplementedException();
        }
    }
}