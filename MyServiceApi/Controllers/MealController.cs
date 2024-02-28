using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyServiceApi.Models;
using MyServiceApi.Services.MenuService;

namespace MyServiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealController : ControllerBase
    {
        private readonly IMenuController _menuController;

        public MealController(IMenuController menuController)
        {
            _menuController = menuController;
        }

        // GET: api/Meal
        [HttpGet]
        public async Task<ActionResult<ServerResponse<MealResponse>>> GetMealRecommendation(
            [Required, FromQuery(Name = "SourceType")] SourceType sourceType,
            [Required, FromQuery(Name = "MealName1")] string mealName1,
            [Required, FromQuery(Name = "CourseType1")] CourseType courseType1,
            [FromQuery(Name = "MealName2")] string mealName2 = "",
            [FromQuery(Name = "CourseType2")] CourseType? courseType2 = null)
        {
            ServerResponse<MealResponse> serverResponse = new();

            try
            {
                MealRequest mealRequest = new() { SourceType = sourceType };
                mealRequest.Meals.Add(new Meal { Name = mealName1, Course = courseType1 });

                if (mealName2.IsNullOrEmpty() && courseType2.HasValue || !mealName2.IsNullOrEmpty() && !courseType2.HasValue)
                {
                    throw new Exception("A required parameter is missing for the second meal");
                }

                if (!mealName2.IsNullOrEmpty() && courseType2.HasValue)
                {
                    if (courseType1.Equals(courseType2))
                        throw new Exception("The course type of the two meals cannot match");

                    mealRequest.Meals.Add(new Meal { Name = mealName2, Course = courseType2 });
                }

                serverResponse = await _menuController.GenerateMealRecommendation(mealRequest);
            }
            catch (Exception e)
            {
                serverResponse.Success = false;
                serverResponse.Message = e.Message;
                return BadRequest(serverResponse);
            }
            return Ok(serverResponse);
        }
    }
}
