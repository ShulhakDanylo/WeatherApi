using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Weather.BLL.DTOs.Weathers;
using Weather.BLL.Helpers;
using Weather.DAL.Data;

namespace Weather.BLL.Queries.GetCurrentWeather;

public class GetCurrentWeatherHandler(
    WeatherDbContext context, 
    IMapper mapper) : IRequestHandler<GetCurrentWeatherQuery, Result<CurrentWeatherDto>>
{
    public async Task<Result<CurrentWeatherDto>> Handle(GetCurrentWeatherQuery request, CancellationToken ct)
    {
        
        var weather = await context.WeatherRecords
            .Include(r => r.Region)
            .Where(r => r.Region.Name == request.RegionName)
            .OrderByDescending(r => r.RecordedAt)
            .FirstOrDefaultAsync(ct);

        if (weather == null) 
            return Result<CurrentWeatherDto>.Failure("Місто не знайдено");
        
        var today = weather.RecordedAt.Date;
        var stats = await context.WeatherRecords
            .Where(r => r.RegionId == weather.RegionId && r.RecordedAt.Date == today)
            .GroupBy(r => r.RegionId)
            .Select(g => new 
            { 
                Min = g.Min(x => x.Temperature), 
                Max = g.Max(x => x.Temperature) 
            })
            .FirstOrDefaultAsync(ct);

        
        var response = mapper.Map<CurrentWeatherDto>(weather, opt => 
        {
            opt.Items["MinTemp"] = Math.Round(stats?.Min ?? weather.Temperature, 0);
            opt.Items["MaxTemp"] = Math.Round(stats?.Max ?? weather.Temperature, 0);
        });

        return Result<CurrentWeatherDto>.Success(response);
    }
}