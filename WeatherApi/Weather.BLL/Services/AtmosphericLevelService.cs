using Microsoft.EntityFrameworkCore;
using Weather.BLL.Helpers;
using Weather.DAL.Data;
using Weather.DAL.Entities;

namespace Weather.BLL.Services;

public class AtmosphericLevelService
{
    private readonly WeatherDbContext _context;
    
    // Коефіцієнт дифузії вологи 
    private const double Kh = 15.0; 

    public AtmosphericLevelService(WeatherDbContext context)
    {
        _context = context;
    }

    public void CalculateAndSaveAtmosphericDataAsync(double dt, List<WeatherRecord> nextData, List<WeatherRecord> currentData)
{
    int n = nextData.Count;
    double[,] matrixA = new double[n, n];
    double[] vectorB_hum = new double[n];

    // Коефіцієнт швидкості адаптації вологості до навколишнього середовища (інерція повітря)
    double relaxationRate = 0.02; 

    for (int i = 0; i < n; i++)
    {
        var current = currentData[i];
        double hour = current.RecordedAt.Hour + (current.RecordedAt.Minute / 60.0);
        int dayOfYear = current.RecordedAt.DayOfYear;

        // 1. ДИНАМІЧНА РІВНОВАГА 
        double targetHumidity = 68.0 - (current.Temperature - 14.0) * 1.2;
        
        if (current.PrecipitationProbability > 0)
        {
            double rainFactor = current.PrecipitationProbability / 100.0;
            targetHumidity = targetHumidity * (1.0 - rainFactor) + 85.0 * rainFactor;
        }
        
        double regionalPhase = (current.Region.Lat * 1.5) + (current.Region.Lon * 0.8);
        double frontWave = Math.Sin((dayOfYear + hour / 24.0) * 2.0 * Math.PI / 4.0 + regionalPhase);
        targetHumidity += frontWave * 8.0;
        
        targetHumidity = Math.Clamp(targetHumidity, 46.0, 88.0);

        // 2. ДИФЕРЕНЦІАЛЬНІ ОПЕРАТОРИ 
        var (dhumdx, dhumdy) = Calculate2DGradients(i, currentData, r => (double)r.Humidity);
        double laplacianHum = Calculate2DLaplacian(i, currentData);
        
        double advectionHum = -(current.WindU * dhumdx + current.WindV * dhumdy) * 100.0;
        double diffusionHum = 20000.0 * laplacianHum; 

        // 3. ФОРМУВАННЯ СИСТЕМИ РІВНЯНЬ 
        matrixA[i, i] = 1.0 + dt * relaxationRate;
        
        vectorB_hum[i] = current.Humidity + dt * (advectionHum + diffusionHum + relaxationRate * targetHumidity);
    }

    double[] initialGuess = currentData.Select(r => (double)r.Humidity).ToArray();
    double[] nextHum = LinearSolver.SolveGaussSeidel(matrixA, vectorB_hum, initialGuess);

    for (int i = 0; i < n; i++)
    {
        double hum = Math.Clamp(nextHum[i], 38.0, 94.0);
        nextData[i].Humidity = (int)Math.Round(hum);
        
        double cloudFactor = (hum - 40.0) / 54.0; 
        nextData[i].Cloudiness = (int)(Math.Pow(Math.Clamp(cloudFactor, 0, 1), 2.0) * 100);

        if (hum > 68.0)
        {
            double delta = hum - 68.0;
            double baseProb = Math.Pow(delta, 1.3) * 1.8;

            double pressureDeficit = Math.Max(0, 101325.0 - currentData[i].Pressure) / 100.0;
            baseProb += pressureDeficit * 0.3;

            nextData[i].PrecipitationProbability = (int)Math.Clamp(baseProb, 0, 90);
        }
        else
        {
            nextData[i].PrecipitationProbability = 0;
        }
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

        return ((east?.Humidity ?? curr.Humidity) + (west?.Humidity ?? curr.Humidity) + 
                (north?.Humidity ?? curr.Humidity) + (south?.Humidity ?? curr.Humidity) - 4 * curr.Humidity) / (d * d);
    }

    private (double dx, double dy) Calculate2DGradients(int idx, List<WeatherRecord> nodes, Func<WeatherRecord, double> property)
    {
        var curr = nodes[idx];
        var east = nodes.Where(n => n.Region.Lon > curr.Region.Lon && Math.Abs(n.Region.Lat - curr.Region.Lat) < 0.6).OrderBy(n => n.Region.Lon).FirstOrDefault();
        var north = nodes.Where(n => n.Region.Lat > curr.Region.Lat && Math.Abs(n.Region.Lon - curr.Region.Lon) < 0.6).OrderBy(n => n.Region.Lat).FirstOrDefault();

        double gradX = 0, gradY = 0;
        if (east != null) gradX = (property(east) - property(curr)) / Math.Max((east.Region.Lon - curr.Region.Lon) * 73000, 30000);
        if (north != null) gradY = (property(north) - property(curr)) / Math.Max((north.Region.Lat - curr.Region.Lat) * 111000, 30000);
        return (gradX, gradY);
    }
}