using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyServiceApi.Data;
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

        // POST: api/Meal
        [HttpPost]
        public async Task<ActionResult<ServerResponse<MealResponse>>> GetMealRecommendation(MealRequest mealRequest)
        {
            ServerResponse<MealResponse> serviceResponse = await _menuController.GenerateMealRecommendation(mealRequest);

            return Ok(serviceResponse);
        }
    }
}
