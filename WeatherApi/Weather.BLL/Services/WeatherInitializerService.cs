using System.Net.Http.Json;
using Weather.DAL.Data;
using Weather.DAL.Entities;
using Weather.BLL.DTOs.Weathers;
using Microsoft.EntityFrameworkCore;


namespace Weather.BLL.Services;

public class WeatherInitializerService
{
    private readonly HttpClient _httpClient;
    private readonly WeatherDbContext _context; 

    public WeatherInitializerService(HttpClient httpClient, WeatherDbContext context)
    {
        _httpClient = httpClient;
        _context = context;
    }

   public async Task InitializeDatabaseAsync()
{
    var systemUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == "system@weather.com");
    if (systemUser == null)
    {
        systemUser = new User { Name = "SystemSolver", Email = "system@weather.com", CreatedAt = DateTime.UtcNow };
        _context.Users.Add(systemUser);
        await _context.SaveChangesAsync();
    }

    if (!_context.Regions.Any())
    {
        double minLat = 46.4, maxLat = 50.5;
        double minLon = 24.0, maxLon = 30.7;
        double step = 0.5; 

        for (double lat = minLat; lat <= maxLat; lat += step)
        {
            for (double lon = minLon; lon <= maxLon; lon += step)
            {
                _context.Regions.Add(new Region 
                { 
                    Name = $"Grid_{lat:F1}_{lon:F1}", 
                    Country = "Ukraine", 
                    Lat = lat, 
                    Lon = lon, 
                    TimeZone = "GMT+3" 
                });
            }
        }
        await _context.SaveChangesAsync();
    }

    var regions = await _context.Regions.ToListAsync();
    foreach (var region in regions)
    {
        var url = $"https://api.open-meteo.com/v1/forecast?latitude={region.Lat.ToString().Replace(',', '.')}&longitude={region.Lon.ToString().Replace(',', '.')}&current=temperature_2m,relative_humidity_2m,surface_pressure,wind_speed_10m,wind_direction_10m&wind_speed_unit=ms";
        
        try {
            var response = await _httpClient.GetFromJsonAsync<OpenMeteoResponse>(url);
            if (response != null)
            {
                double angleRad = response.current.wind_direction_10m * Math.PI / 180.0;
                _context.WeatherRecords.Add(new WeatherRecord
                {
                    RegionId = region.Id,
                    UserId = systemUser.Id,
                    RecordedAt = DateTime.UtcNow,
                    Temperature = response.current.temperature_2m,
                    Pressure = response.current.surface_pressure * 100, 
                    WindU = -response.current.wind_speed_10m * Math.Sin(angleRad),
                    WindV = -response.current.wind_speed_10m * Math.Cos(angleRad),
                    WindSpeed = response.current.wind_speed_10m,
                    Humidity = response.current.relative_humidity_2m,
                    IsForecast = false
                });
            }
        } catch { /* Пропуск битих точок */ }
    }
    await _context.SaveChangesAsync();
}
}