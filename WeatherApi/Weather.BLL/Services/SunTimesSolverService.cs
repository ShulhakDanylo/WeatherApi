using CoordinateSharp;

namespace Weather.BLL.Services;

public class SunTimesSolverService
{
    public (string Sunrise, string Sunset) CalculateSunTimes(string gridName, DateTime date)
    {
        string cityName = MapGridToCity(gridName);

        double lat = 49.8397;
        double lon = 24.0297;

        switch (cityName)
        {
            case "Львів":
                lat = 49.8397; 
                lon = 24.0297;
                break;
            case "Солонка":
                lat = 49.7523; 
                lon = 24.0321;
                break;
            case "Київ":
                lat = 50.4501; 
                lon = 30.5234;
                break;
        }
        
        Celestial celestial = Celestial.CalculateCelestialTimes(lat, lon, date);

        TimeZoneInfo ukraineTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. Europe Standard Time");

        string sunrise = "06:00";
        string sunset = "21:00";

        if (celestial.SunRise.HasValue)
        {
            DateTime localSunrise = TimeZoneInfo.ConvertTimeFromUtc(celestial.SunRise.Value, ukraineTimeZone);
            sunrise = localSunrise.ToString("HH:mm");
        }

        if (celestial.SunSet.HasValue)
        {
            DateTime localSunset = TimeZoneInfo.ConvertTimeFromUtc(celestial.SunSet.Value, ukraineTimeZone);
            sunset = localSunset.ToString("HH:mm");
        }

        return (sunrise, sunset);
    }

    public string MapGridToCity(string gridName)
    {
        return gridName switch
        {
            "Grid_46,4_24,0" => "Львів",
            "Grid_46,4_24,5" => "Солонка",
            "Grid_46,4_25,0" => "Київ",
            _ => "Львів"
        };
    }
}