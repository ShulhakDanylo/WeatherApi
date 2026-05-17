using Microsoft.EntityFrameworkCore;
using Weather.DAL.Data; 
using Weather.DAL.Entities; 
using FluentValidation;
using Weather.BLL.Validators.User;
using Weather.BLL.Commands.Users.Register;
using Weather.BLL.Mappers;
using Weather.BLL.Interfaces;
using Weather.BLL.Services;
using Weather.DAL.Repositories.Interfaces.Base;
using Weather.DAL.Repositories.Realizations.Base;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly));
builder.Services.AddAutoMapper(typeof(UserProfile).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(RegisterUserValidator).Assembly);
builder.Services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
builder.Services.AddScoped<ITokenService, TokenService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173") 
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


builder.Services.AddDbContext<WeatherDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddHttpClient();
builder.Services.AddScoped<WeatherInitializerService>();
builder.Services.AddScoped<WindSolverService>();
builder.Services.AddScoped<PressureSolverService>();
builder.Services.AddScoped<TemperatureSolverService>();
builder.Services.AddScoped<AtmosphericLevelService>();
builder.Services.AddScoped<ForecastGeneratorService>();
builder.Services.AddScoped<SunTimesSolverService>();
builder.Services.AddDbContext<WeatherDbContext>(options =>
{
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.MigrationsAssembly(typeof(WeatherDbContext).Assembly.FullName);
    });
});
var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<WeatherDbContext>();

    try
    {
        var initializer = services.GetRequiredService<WeatherInitializerService>();
        await initializer.InitializeDatabaseAsync();
        Console.WriteLine("База ініціалізована реальними даними з API.");
        
        var currentData = await context.WeatherRecords
            .AsNoTracking()
            .Include(r => r.Region)
            .Where(r => !r.IsForecast)
            .ToListAsync();

        if (!currentData.Any()) throw new Exception("Початкові дані не знайдені.");

        var windSolver = services.GetRequiredService<WindSolverService>();
        var pressureSolver = services.GetRequiredService<PressureSolverService>();
        var temperatureSolver = services.GetRequiredService<TemperatureSolverService>();
        var atmosphericService = services.GetRequiredService<AtmosphericLevelService>();
        var sunTimesService = services.GetRequiredService<SunTimesSolverService>();
        
        DateTime now = DateTime.UtcNow;
        DateTime forecastTime = new DateTime(now.Year, now.Month, now.Day, (now.Hour / 3) * 3, 0, 0, DateTimeKind.Utc)
            .AddHours(3);

        double dt = 300; 
        int stepsPerHour = 3600 / (int)dt;
        int totalHours = 168; 

        Console.WriteLine($"--- СТАРТ ГЕНЕРАЦІЇ ПРОГНОЗУ НА 7 ДНІВ ---");
        
        var runningData = currentData.Select(source => new WeatherRecord
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

        for (int h = 1; h <= totalHours; h++)
        {
            forecastTime = forecastTime.AddHours(1);

            
            foreach (var record in runningData)
            {
                record.RecordedAt = forecastTime;
                var (sunrise, sunset) = sunTimesService.CalculateSunTimes(record.Region!.Name, forecastTime);
        
                TimeSpan sunriseTime = TimeSpan.Parse(sunrise);
                TimeSpan sunsetTime = TimeSpan.Parse(sunset);
                TimeSpan currentHour = forecastTime.TimeOfDay;

                bool isNight = currentHour < sunriseTime || currentHour > sunsetTime;
            }

            for (int sub = 0; sub < stepsPerHour; sub++)
            {
               
                var subNextData = runningData.Select(r => new WeatherRecord
                {
                    RegionId = r.RegionId,
                    Region = r.Region,
                    RecordedAt = r.RecordedAt,
                    IsForecast = r.IsForecast,
                    UserId = r.UserId,
                    Temperature = r.Temperature,
                    Pressure = r.Pressure,
                    WindU = r.WindU,
                    WindV = r.WindV,
                    WindSpeed = r.WindSpeed,
                    Humidity = r.Humidity,
                    Cloudiness = r.Cloudiness,
                    PrecipitationProbability = r.PrecipitationProbability,
                    FeelsLike = r.FeelsLike
                }).ToList();

                
                await windSolver.CalculateWindStepAsync(dt, subNextData);

               
                pressureSolver.CalculateAndSavePressureAsync(dt, subNextData, runningData);
                temperatureSolver.CalculateAndSaveTemperatureAsync(dt, subNextData, runningData);
                atmosphericService.CalculateAndSaveAtmosphericDataAsync(dt, subNextData, runningData);

                
                runningData = subNextData;
            }

            if (forecastTime.Hour % 3 == 0)
            {
                
                var dataToSave = runningData.Select(r => new WeatherRecord
                {
                    RegionId = r.RegionId,
                    UserId = r.UserId,
                    RecordedAt = r.RecordedAt,
                    IsForecast = true,
                    Temperature = r.Temperature,
                    Pressure = r.Pressure,
                    WindU = r.WindU,
                    WindV = r.WindV,
                    WindSpeed = r.WindSpeed,
                    Humidity = r.Humidity,
                    Cloudiness = r.Cloudiness,
                    PrecipitationProbability = r.PrecipitationProbability,
                    FeelsLike = r.FeelsLike
                }).ToList();

                await context.WeatherRecords.AddRangeAsync(dataToSave);
                await context.SaveChangesAsync();

                context.ChangeTracker.Clear();

                Console.WriteLine($"> [SAVE] {forecastTime:dd.MM HH:mm} | " +
                                  $"Середня Т: {runningData.Average(x => x.Temperature):F1}°C | " +
                                  $"Середня Волог: {runningData.Average(x => x.Humidity):F0}% | " +
                                  $"Вітер: {runningData.Average(x => x.WindSpeed):F1} м/с");
            }
        }

        Console.WriteLine("--- ПРОГНОЗ НА 7 ДНІВ ЗАВЕРШЕНО УСПІШНО ---");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Критична помилка розрахунку: {ex.Message}");
        if (ex.StackTrace != null) Console.WriteLine(ex.StackTrace);
    }
}

app.UseHttpsRedirection();

app.UseCors("ReactPolicy");
app.MapControllers();
app.Run();
