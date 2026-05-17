namespace Weather.BLL.DTOs.Weathers;

public class CurrentData
{
    public double temperature_2m { get; set; }
    public double surface_pressure { get; set; }
    public double wind_speed_10m { get; set; }
    public int wind_direction_10m { get; set; }
    public int relative_humidity_2m { get; set; }
}