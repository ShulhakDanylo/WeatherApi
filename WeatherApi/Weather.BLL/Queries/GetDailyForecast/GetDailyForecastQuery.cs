using MediatR;
using Weather.BLL.Helpers;
using Weather.BLL.DTOs.Weathers;

namespace Weather.BLL.Queries.GetDailyForecast;

public record GetDailyForecastQuery(string RegionName) : 
    IRequest<Result<IEnumerable<DailyForecastDto>>>;