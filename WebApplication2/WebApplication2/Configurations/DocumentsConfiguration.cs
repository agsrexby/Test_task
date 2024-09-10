using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication2.Models;

namespace WebApplication2.Configurations;

public class DocumentsConfiguration : IEntityTypeConfiguration<InfoOfDocInCar>
{
    public void Configure(EntityTypeBuilder<InfoOfDocInCar> builder)
    {
        builder.HasKey(e => e.Id);

        builder
            .HasOne(e => e.RefuelDoc)
            .WithMany(e => e.Documents)
            .HasForeignKey(e => e.RefuelingId);
    }
}