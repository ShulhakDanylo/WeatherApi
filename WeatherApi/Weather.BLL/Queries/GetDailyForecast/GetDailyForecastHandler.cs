using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Weather.BLL.DTOs.Weathers;
using Weather.BLL.Helpers;
using Weather.DAL.Data;

namespace Weather.BLL.Queries.GetDailyForecast;

public class GetDailyForecastHandler(WeatherDbContext context, IMapper mapper) 
    : IRequestHandler<GetDailyForecastQuery, Result<IEnumerable<DailyForecastDto>>>
{
    public async Task<Result<IEnumerable<DailyForecastDto>>> Handle(GetDailyForecastQuery request, CancellationToken ct)
    {
        
        var forecastRecords = await context.WeatherRecords
            .Include(r => r.Region)
            .Where(r => r.Region!.Name == request.RegionName && r.RecordedAt.Date >= DateTime.UtcNow.Date)
            .OrderBy(r => r.RecordedAt)
            .ToListAsync(ct);

        if (!forecastRecords.Any()) 
            return Result<IEnumerable<DailyForecastDto>>.Failure("Прогноз не знайдено");

        
        var grouped = forecastRecords
            .GroupBy(r => r.RecordedAt.Date)
            .Take(7); 

       
        var response = mapper.Map<IEnumerable<DailyForecastDto>>(grouped);

        return Result<IEnumerable<DailyForecastDto>>.Success(response);
    }
}