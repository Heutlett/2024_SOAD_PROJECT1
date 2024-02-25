using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyServiceApi.Data;
using MyServiceApi.Models;
using MyServiceApi.Services.MealService;

namespace MyServiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MealController : ControllerBase
    {
        private readonly IMealService _mealService;

        public MealController(IMealService mealService)
        {
            _mealService = mealService;
        }

        // GET: api/Meal
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<Meal>>>> GetMealItems()
        {
            ServiceResponse<List<Meal>> serviceResponse = await _mealService.ReadMeals();

            return Ok(serviceResponse);
        }

        // POST: api/Meal
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ServiceResponse<Meal>>> PostMeal(CreateMealDto newMeal)
        {
            ServiceResponse<Meal> serviceResponse = await _mealService.CreateMeal(newMeal);

            return Ok(serviceResponse);
        }

        //     // GET: api/Meal/5
        //     [HttpGet("{id}")]
        //     public async Task<ActionResult<Meal>> GetMeal(long id)
        //     {
        //         var meal = await _context.MealItems.FindAsync(id);

        //         if (meal == null)
        //         {
        //             return NotFound();
        //         }

        //         return meal;
        //     }

        //     // PUT: api/Meal/5
        //     // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //     [HttpPut("{id}")]
        //     public async Task<IActionResult> PutMeal(long id, Meal meal)
        //     {
        //         if (id != meal.Id)
        //         {
        //             return BadRequest();
        //         }

        //         _context.Entry(meal).State = EntityState.Modified;

        //         try
        //         {
        //             await _context.SaveChangesAsync();
        //         }
        //         catch (DbUpdateConcurrencyException)
        //         {
        //             if (!MealExists(id))
        //             {
        //                 return NotFound();
        //             }
        //             else
        //             {
        //                 throw;
        //             }
        //         }

        //         return NoContent();
        //     }



        //     // DELETE: api/Meal/5
        //     [HttpDelete("{id}")]
        //     public async Task<IActionResult> DeleteMeal(long id)
        //     {
        //         var meal = await _context.MealItems.FindAsync(id);
        //         if (meal == null)
        //         {
        //             return NotFound();
        //         }

        //         _context.MealItems.Remove(meal);
        //         await _context.SaveChangesAsync();

        //         return NoContent();
        //     }

        //     private bool MealExists(long id)
        //     {
        //         return _context.MealItems.Any(e => e.Id == id);
        //     }
    }
}
