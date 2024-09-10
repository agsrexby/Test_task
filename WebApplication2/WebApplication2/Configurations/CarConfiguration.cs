using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication2.Models;

namespace WebApplication2.Configurations;

public class CarConfiguration : IEntityTypeConfiguration<Car>
{
    public void Configure(EntityTypeBuilder<Car> builder)
    {
        builder.HasKey(a => a.Id);

        builder
            .HasMany(a => a.Refuels)
            .WithOne(c => c.Car)
            .HasForeignKey(e => e.CarId);

    }
}