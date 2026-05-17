using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Weather.DAL.Entities;

namespace Weather.DAL.Data.Configurations;

public class WeatherRecordConfiguration : IEntityTypeConfiguration<WeatherRecord>
{
    public void Configure(EntityTypeBuilder<WeatherRecord> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd();

        builder.Property(x => x.RegionId)
            .IsRequired();

        builder.Property(x => x.RecordedAt)
            .IsRequired();

        // Основні показники
        builder.Property(x => x.WindSpeed)
            .IsRequired();
        builder.Property(x => x.WindU).IsRequired()
            .HasDefaultValue(0);
        builder.Property(x => x.WindV).IsRequired()
            .HasDefaultValue(0);
        
        builder.Property(x => x.Temperature)
            .IsRequired();

        builder.Property(x => x.Humidity)
            .IsRequired();

        builder.Property(x => x.Pressure)
            .IsRequired();

        builder.Property(x => x.PrecipitationProbability)
            .IsRequired();

        builder.Property(x => x.Cloudiness)
            .IsRequired();

        builder.Property(x => x.FeelsLike)
            .IsRequired();
        
        builder.Property(x => x.IsForecast).IsRequired()
            .HasDefaultValue(false);

      
        builder.HasIndex(x => new { x.RegionId, x.RecordedAt, x.IsForecast });

       
        builder.HasOne(x => x.Region)
            .WithMany()
            .HasForeignKey(x => x.RegionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.User)
            .WithMany() 
            .HasForeignKey(x => x.UserId);
    }
}