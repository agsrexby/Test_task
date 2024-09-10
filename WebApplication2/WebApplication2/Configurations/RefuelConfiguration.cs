using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication2.Models;

namespace WebApplication2.Configurations;

public class RefuelConfiguration : IEntityTypeConfiguration<Refueling>
{
    public void Configure(EntityTypeBuilder<Refueling> builder)
    {
        builder.HasKey(a => a.Id);

        builder
            .HasOne(a => a.Car)
            .WithMany(c => c.Refuels)
            .HasForeignKey(e => e.CarId);

        builder
            .HasMany(e => e.Documents)
            .WithOne(e => e.RefuelDoc)
            .HasForeignKey(e => e.RefuelingId);

        builder
            .HasMany(e => e.Photos)
            .WithOne(e => e.Refuel)
            .HasForeignKey(e => e.RefuelingId);
    }
}