using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Weather.DAL.Entities;

namespace Weather.DAL.Data.Configurations;

public class RegionConfiguration : IEntityTypeConfiguration<Region>
{
    public void Configure(EntityTypeBuilder<Region> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Country)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Lat)
            .IsRequired();

        builder.Property(x => x.Lon)
            .IsRequired();

        builder.Property(x => x.TimeZone)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(x => new { x.Name, x.Country })
            .IsUnique();
    }
}