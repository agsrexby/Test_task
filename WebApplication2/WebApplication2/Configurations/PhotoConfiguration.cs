using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication2.Models;

namespace WebApplication2.Configurations;

public class PhotoConfiguration : IEntityTypeConfiguration<Photo>
{
    public void Configure(EntityTypeBuilder<Photo> builder)
    {
        builder.HasKey(e => e.Id);

        builder
            .HasOne(e => e.Refuel)
            .WithMany(e => e.Photos)
            .HasForeignKey(e => e.RefuelingId);
    }
}