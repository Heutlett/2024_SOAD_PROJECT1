using Microsoft.EntityFrameworkCore;
using MyServiceApi.Models;

namespace MyServiceApi.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    public DbSet<Meal> MealItems { get; set; }
}