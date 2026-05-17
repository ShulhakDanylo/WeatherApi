// Weather.BLL.Mappers/WeatherProfile.cs
using AutoMapper;
using Weather.DAL.Entities;
using Weather.BLL.DTOs.Weathers;
using Weather.BLL.Services;

namespace Weather.BLL.Mappers;

public class WeatherProfile : Profile
{
    public WeatherProfile()
    {
        CreateMap<WeatherRecord, CurrentWeatherDto>()
            .ForMember(d => d.RegionName, opt => opt.MapFrom(s => MapGridToCity(s.Region.Name)))
            .ForMember(d => d.Temperature, opt => opt.MapFrom(s => Math.Round(s.Temperature, 1)))
            .ForMember(d => d.FeelsLike, opt => opt.MapFrom(s => Math.Round(s.FeelsLike, 1)))
            .ForMember(d => d.Pressure, opt => opt.MapFrom(s => Math.Round(s.Pressure / 133.322, 0)))
            
            
            .ForMember(d => d.Condition, opt => opt.MapFrom((src, dest, destMember, context) => 
            {
                var sunService = new SunTimesSolverService();
                var (sunrise, sunset) = sunService.CalculateSunTimes(src.Region.Name, src.RecordedAt);
                TimeSpan sunriseTime = TimeSpan.Parse(sunrise);
                TimeSpan sunsetTime = TimeSpan.Parse(sunset);
                bool isNight = src.RecordedAt.TimeOfDay < sunriseTime || src.RecordedAt.TimeOfDay > sunsetTime;

                return GetCondition(src.Cloudiness, src.PrecipitationProbability > 50 ? 0.5 : 0, isNight);
            }))
            
            
            .ForMember(d => d.MinTempToday, opt => opt.MapFrom((src, _, _, context) => 
                context.TryGetItems(out var items) && items.TryGetValue("MinTemp", out var min) ? min : src.Temperature))
            .ForMember(d => d.MaxTempToday, opt => opt.MapFrom((src, _, _, context) => 
                context.TryGetItems(out var items) && items.TryGetValue("MaxTemp", out var max) ? max : src.Temperature))
            
            .ForMember(d => d.Precipitation, opt => opt.MapFrom(s => s.PrecipitationProbability > 0 ? 0.5 : 0))
            .ForMember(d => d.PrecipitationDescription, opt => opt.MapFrom(_ => "Очікується за останні 24 год"))
            .ForMember(d => d.FeelsLikeDescription, opt => opt.MapFrom(s => s.WindSpeed > 4 ? "Через вітер здається холодніше" : "Показник відповідає нормі"))
            .ForMember(d => d.PressureStatus, opt => opt.MapFrom(_ => "Стабільний протягом дня"))
            .ForMember(d => d.CloudinessDescription, opt => opt.MapFrom(s => s.Cloudiness > 50 ? "Переважно похмуро" : "Сьогодні дуже ясно"))
            
            
            .ForMember(d => d.Sunrise, opt => opt.MapFrom((src, dest, destMember, context) => 
                new SunTimesSolverService().CalculateSunTimes(src.Region.Name, src.RecordedAt).Sunrise))
            .ForMember(d => d.Sunset, opt => opt.MapFrom((src, dest, destMember, context) => 
                new SunTimesSolverService().CalculateSunTimes(src.Region.Name, src.RecordedAt).Sunset));

        
        CreateMap<IGrouping<DateTime, WeatherRecord>, DailyForecastDto>()
            .ConstructUsing((g, context) => CalculateDailyForecastInternal(g, new SunTimesSolverService()));
    }

    private static DailyForecastDto CalculateDailyForecastInternal(IGrouping<DateTime, WeatherRecord> g, SunTimesSolverService sunService)
    {
        var firstRecord = g.First();
        var cityName = firstRecord.Region.Name switch
        {
            "Grid_46,4_24,0" => "Львів",
            "Grid_46,4_24,5" => "Солонка",
            "Grid_46,4_25,0" => "Київ",
            _ => "Львів"
        };
        
        var (sunrise, sunset) = sunService.CalculateSunTimes(firstRecord.Region.Name, g.Key);
        
        TimeSpan sunriseTime = TimeSpan.Parse(sunrise);
        TimeSpan sunsetTime = TimeSpan.Parse(sunset);

        double dayRain = g.Sum(x => x.PrecipitationProbability > 50 ? 0.5 : 0);

        return new DailyForecastDto(
            GetDayName(g.Key),
            Math.Round(g.Min(x => x.Temperature), 0),
            Math.Round(g.Max(x => x.Temperature), 0),
            g.Key,
            Math.Round(g.Average(x => x.WindSpeed), 1),
            GetWindDirection(g.Average(x => x.WindU), g.Average(x => x.WindV)),
            Math.Round(dayRain, 1),
            (int)g.Average(x => x.Humidity),
            Math.Round(g.Average(x => x.Pressure / 100), 0),
            15.0,
            sunrise,
            sunset,
            StaticGetCondition((int)g.Average(x => x.Cloudiness), dayRain, false),
            
            g.OrderBy(x => x.RecordedAt).Select(x => {
                var hourTime = x.RecordedAt.TimeOfDay;
                bool isNight = hourTime < sunriseTime || hourTime > sunsetTime;

                return new HourlyForecastDto(
                    x.RecordedAt.ToString("HH:mm"), 
                    Math.Round(x.Temperature, 1),
                    StaticGetCondition(x.Cloudiness, x.PrecipitationProbability > 50 ? 0.5 : 0, isNight)
                );
            }).ToList()
        );
    }

    private static string StaticGetCondition(int clouds, double rain, bool isNight)
    {
        string prefix = isNight ? "Ніч, " : "";
        if (rain >= 1.0 && clouds > 40) return prefix + "Гроза";
        if (rain > 0.0) return clouds <= 60 ? (isNight ? "Ніч, мінлива хмарність, дощ" : "Мінлива хмарність, дощ") : prefix + "Дощ";
        if (clouds > 80) return prefix + "Похмуро";
        return clouds > 15 ? (isNight ? "Ніч, мінлива хмарність" : "Мінлива хмарність") : (isNight ? "Ніч, Ясно" : "Сонячно");
    }

    private string GetCondition(int clouds, double rain, bool isNight) => StaticGetCondition(clouds, rain, isNight);

    private string MapGridToCity(string gridName) => gridName switch
    {
        "Grid_46,4_24,0" => "Львів",
        "Grid_46,4_24,5" => "Солонка",
        "Grid_46,4_25,0" => "Київ",
        _ => "Львів"
    };

    private static string GetDayName(DateTime date)
    {
        if (date.Date == DateTime.UtcNow.Date) return "Сьогодні";
        return new System.Globalization.CultureInfo("uk-UA").DateTimeFormat.GetAbbreviatedDayName(date.DayOfWeek).ToUpper();
    }

    private static string GetWindDirection(double u, double v)
    {
        if (Math.Abs(u) > Math.Abs(v)) return u > 0 ? "Схід" : "Захід";
        return v > 0 ? "Північ" : "Південь";
    }
}