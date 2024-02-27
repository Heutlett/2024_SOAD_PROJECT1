using Microsoft.EntityFrameworkCore;
using MyServiceApi.Models;

namespace MyServiceApi.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }
    // Implementar sistema de parseo de meals desde json
    public DbSet<Meal> MealItems { get; set; }
}