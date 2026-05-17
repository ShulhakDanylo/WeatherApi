using Microsoft.EntityFrameworkCore;
using Weather.BLL.Helpers;
using Weather.DAL.Data;
using Weather.DAL.Entities;

namespace Weather.BLL.Services;

public class TemperatureSolverService
{
    private readonly WeatherDbContext _context;
    private const double Kh = 8.0; // Дифузія для згладжування шуму

    public TemperatureSolverService(WeatherDbContext context)
    {
        _context = context;
    }

    public void CalculateAndSaveTemperatureAsync(double dt, List<WeatherRecord> nextData, List<WeatherRecord> currentData)
    {
        int n = nextData.Count;

        for (int i = 0; i < n; i++)
        {
            var current = currentData[i];
            double hour = current.RecordedAt.Hour + (current.RecordedAt.Minute / 60.0);
            int dayOfYear = current.RecordedAt.DayOfYear;
            
            double sunFactor = Math.Cos((hour - 14.5) * Math.PI / 12.0);

            var (dtdx, dtdy) = Calculate2DGradients(i, currentData, r => r.Temperature);
            double laplacianT = Calculate2DLaplacian(i, currentData);

            // 2. ФІЗИКА (Адвекція та дифузія)
            double advectionT = -(current.WindU * dtdx + current.WindV * dtdy);
            double diffusionT = Kh * laplacianT;

            double cloudFactor = current.Cloudiness / 100.0;
            
            double solarHeating = 0.0011 * Math.Max(0, sunFactor) * (1.0 - 0.4 * cloudFactor);
            double nightCooling = -0.0016 * (1.0 - Math.Max(0, sunFactor));
            double radiation = solarHeating + nightCooling;
            
            double dailyClimateWalk = 14.5 + (sunFactor * 4.5); 
      
            double synopticDayWave = Math.Sin(dayOfYear * 2.0 * Math.PI / 5.0) * 1.2;
            double regionalBase = dailyClimateWalk - (current.Region.Lat - 49.8) * 1.0 + (current.Region.Lon - 24.0) * 0.2 + synopticDayWave;
            
            double relaxation = (regionalBase - current.Temperature) * 0.0035;

            double dT_dt = advectionT + (diffusionT * 0.1) + radiation + relaxation;

            dT_dt = Math.Clamp(dT_dt, -0.0015, 0.0015);

            double nextT = current.Temperature + (dt * dT_dt);

            nextData[i].Temperature = Math.Round(Math.Clamp(nextT, 5.0, 26.0), 1);
        
            double windEffect = Math.Sqrt(Math.Abs(nextData[i].WindSpeed) + 0.1) * 0.4;
            nextData[i].FeelsLike = Math.Round(nextData[i].Temperature - windEffect, 1);
        }
    }

    private double Calculate2DLaplacian(int idx, List<WeatherRecord> nodes)
    {
        var curr = nodes[idx];
        double d = 50000.0;
        var east = nodes.Where(n => n.Region.Lon > curr.Region.Lon && Math.Abs(n.Region.Lat - curr.Region.Lat) < 0.3).OrderBy(n => n.Region.Lon).FirstOrDefault();
        var west = nodes.Where(n => n.Region.Lon < curr.Region.Lon && Math.Abs(n.Region.Lat - curr.Region.Lat) < 0.3).OrderByDescending(n => n.Region.Lon).FirstOrDefault();
        var north = nodes.Where(n => n.Region.Lat > curr.Region.Lat && Math.Abs(n.Region.Lon - curr.Region.Lon) < 0.3).OrderBy(n => n.Region.Lat).FirstOrDefault();
        var south = nodes.Where(n => n.Region.Lat < curr.Region.Lat && Math.Abs(n.Region.Lon - curr.Region.Lon) < 0.3).OrderByDescending(n => n.Region.Lat).FirstOrDefault();

        return ((east?.Temperature ?? curr.Temperature) + (west?.Temperature ?? curr.Temperature) + 
                (north?.Temperature ?? curr.Temperature) + (south?.Temperature ?? curr.Temperature) - 4 * curr.Temperature) / (d * d);
    }

    private (double dx, double dy) Calculate2DGradients(int idx, List<WeatherRecord> nodes, Func<WeatherRecord, double> property)
    {
        var curr = nodes[idx];
        var east = nodes.Where(n => n.Region.Lon > curr.Region.Lon && Math.Abs(n.Region.Lat - curr.Region.Lat) < 0.6).OrderBy(n => n.Region.Lon).FirstOrDefault();
        var north = nodes.Where(n => n.Region.Lat > curr.Region.Lat && Math.Abs(n.Region.Lon - curr.Region.Lon) < 0.6).OrderBy(n => n.Region.Lat).FirstOrDefault();

        double gradX = 0, gradY = 0;
        if (east != null) gradX = (property(east) - property(curr)) / Math.Max((east.Region.Lon - curr.Region.Lon) * 73000, 20000);
        if (north != null) gradY = (property(north) - property(curr)) / Math.Max((north.Region.Lat - curr.Region.Lat) * 111000, 20000);
        return (gradX, gradY);
    }
}