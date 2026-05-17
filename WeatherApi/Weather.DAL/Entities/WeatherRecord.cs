
namespace Weather.DAL.Entities;

public class WeatherRecord
{
    public int Id { get; set; }
    public int RegionId { get; set; }
    public Region? Region { get; set; }

    public DateTime RecordedAt { get; set; } = DateTime.UtcNow; 
    
    public double WindSpeed { get; set; } 
    public double WindU { get; set; }     
    public double WindV { get; set; }    
    
    
    public double Temperature { get; set; }
    public int Humidity { get; set; }
    public double Pressure { get; set; }
    public double PrecipitationProbability { get; set; }
    public int Cloudiness { get; set; }
    public double FeelsLike { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public bool IsForecast { get; set; }  // Чи це розраховано сервісом
    
}