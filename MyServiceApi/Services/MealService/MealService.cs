using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyServiceApi.Data;
using MyServiceApi.Models;

namespace MyServiceApi.Services.MealService
{
    public class MealService : IMealService
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;

        public MealService(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ServiceResponse<Meal>> CreateMeal(CreateMealDto newMeal)
        {
            ServiceResponse<Meal> serviceResponse = new ServiceResponse<Meal>();

            Meal meal = new Meal
            {
                Name = newMeal.Name,
                Course = newMeal.Course
            };

            _context.MealItems.Add(meal);
            await _context.SaveChangesAsync();

            serviceResponse.Data = meal;
            serviceResponse.Success = true;
            serviceResponse.Message = "The meal was created successfully";

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<Meal>>> ReadMeals()
        {
            ServiceResponse<List<Meal>> serviceResponse = new ServiceResponse<List<Meal>>
            {
                Data = await _context.MealItems.ToListAsync(),
                Success = true
            };

            return serviceResponse;
        }

        public ServiceResponse<List<Meal>> GenerateMealRecommendationAI(Meal meal)
        {
            throw new NotImplementedException();
        }

        public ServiceResponse<List<Meal>> GenerateMealRecommendationExternal(Meal meal)
        {
            throw new NotImplementedException();
        }

        public ServiceResponse<List<Meal>> GenerateMealRecommendationNative(Meal meal)
        {
            throw new NotImplementedException();
        }
    }
}