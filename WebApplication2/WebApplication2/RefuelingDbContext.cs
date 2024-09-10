using Microsoft.EntityFrameworkCore;
using WebApplication2.Configurations;
using WebApplication2.Models;

namespace WebApplication2;

public class RefuelingDbContext(DbContextOptions<RefuelingDbContext> options) : DbContext(options)
{
    
    public DbSet<Car> Cars { get; set; }
    public DbSet<Refueling> Refuels { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<InfoOfDocInCar> Documents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CarConfiguration());
        modelBuilder.ApplyConfiguration(new DocumentsConfiguration());
        modelBuilder.ApplyConfiguration(new PhotoConfiguration());
        modelBuilder.ApplyConfiguration(new RefuelConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
    

}