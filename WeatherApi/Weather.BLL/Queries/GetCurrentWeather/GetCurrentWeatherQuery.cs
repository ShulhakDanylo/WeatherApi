using MediatR;
using Weather.BLL.Helpers;
using Weather.BLL.DTOs.Weathers;

namespace Weather.BLL.Queries.GetCurrentWeather;

public record GetCurrentWeatherQuery(string RegionName) : 
    IRequest<Result<CurrentWeatherDto>>;