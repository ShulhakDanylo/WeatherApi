namespace Weather.DAL.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<WeatherRecord> WeatherRecords { get; set; } = new List<WeatherRecord>();
}