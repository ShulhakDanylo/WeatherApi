using Microsoft.EntityFrameworkCore;
using Weather.DAL.Data;
using Weather.DAL.Entities;

namespace Weather.BLL.Services;

public class ForecastGeneratorService
{
    private readonly WeatherDbContext _context;
    private readonly WindSolverService _windSolver;
    private readonly PressureSolverService _pressureSolver;
    private readonly TemperatureSolverService _temperatureSolver;
    private readonly AtmosphericLevelService _atmosphericService;
    private readonly WeatherInitializerService _initializer;

    public ForecastGeneratorService(
        WeatherDbContext context,
        WindSolverService windSolver,
        PressureSolverService pressureSolver,
        TemperatureSolverService temperatureSolver,
        AtmosphericLevelService atmosphericService,
        WeatherInitializerService initializer)
    {
        _context = context;
        _windSolver = windSolver;
        _pressureSolver = pressureSolver;
        _temperatureSolver = temperatureSolver;
        _atmosphericService = atmosphericService;
        _initializer = initializer;
    }

    public async Task Generate7DayForecastAsync()
    {
        try
        {
            await _initializer.InitializeDatabaseAsync();
            Console.WriteLine("База ініціалізована актуальними даними.");

            var currentData = await _context.WeatherRecords
                .AsNoTracking()
                .Include(r => r.Region)
                .Where(r => !r.IsForecast)
                .ToListAsync();

            if (!currentData.Any()) throw new Exception("Початкові дані для прогнозу відсутні.");

            DateTime now = DateTime.UtcNow;
            DateTime forecastTime = new DateTime(now.Year, now.Month, now.Day, (now.Hour / 3) * 3, 0, 0, DateTimeKind.Utc).AddHours(3);

            double dt = 300; 
            int stepsPerHour = 3600 / (int)dt;
            int totalHours = 168; 

            for (int h = 1; h <= totalHours; h++)
            {
                forecastTime = forecastTime.AddHours(1);
                
                var nextData = currentData.Select(source => new WeatherRecord
                {
                    RegionId = source.RegionId,
                    Region = source.Region,
                    RecordedAt = forecastTime,
                    IsForecast = true,
                    UserId = source.UserId,
                    Temperature = source.Temperature,
                    Pressure = source.Pressure,
                    WindU = source.WindU,
                    WindV = source.WindV,
                    WindSpeed = source.WindSpeed,
                    Humidity = source.Humidity,
                    Cloudiness = source.Cloudiness,
                    PrecipitationProbability = source.PrecipitationProbability,
                    FeelsLike = source.FeelsLike
                }).ToList();

                for (int sub = 0; sub < stepsPerHour; sub++)
                {
                    var calculatedWind = await _windSolver.CalculateWindStepAsync(dt, nextData);
                    _pressureSolver.CalculateAndSavePressureAsync(dt, calculatedWind, currentData);
                    _temperatureSolver.CalculateAndSaveTemperatureAsync(dt, calculatedWind, currentData);
                    _atmosphericService.CalculateAndSaveAtmosphericDataAsync(dt, calculatedWind, currentData);

                    currentData = calculatedWind;
                    nextData = calculatedWind;
                }

                if (forecastTime.Hour % 3 == 0)
                {
                    foreach (var record in nextData) { record.Region = null; }

                    await _context.WeatherRecords.AddRangeAsync(nextData);
                    await _context.SaveChangesAsync();
                    
                    _context.ChangeTracker.Clear();

                    foreach (var d in currentData) {
                        d.Region = await _context.Regions.FindAsync(d.RegionId);
                    }

                    Console.WriteLine($"> [PROCESSED] {forecastTime:dd.MM HH:mm} | Avg T: {currentData.Average(x => x.Temperature):F1}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка генерації прогнозу: {ex.Message}");
            throw;
        }
    }
}