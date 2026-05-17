namespace Weather.DAL.Entities;

public class Region
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } =  string.Empty;
    public double Lat { get; set; }
    public double Lon { get; set; }
    public string TimeZone { get; set; } = "UTC"; 
}  