using Microsoft.EntityFrameworkCore;
using Weather.DAL.Entities;

namespace Weather.DAL.Data;

public class WeatherDbContext : DbContext
{
    public WeatherDbContext(DbContextOptions<WeatherDbContext> options)
        : base(options)
    {
    }

    public DbSet<WeatherRecord> WeatherRecords { get; set; }

    public DbSet<Region> Regions { get; set; }
    
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WeatherDbContext).Assembly);
    }
}