using MediatR;
using Microsoft.AspNetCore.Mvc;
using Weather.BLL.DTOs.Weathers;
using Weather.BLL.Queries.GetCurrentWeather;
using Weather.BLL.Queries.GetDailyForecast;

namespace WeatherApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController(IMediator mediator) : ControllerBase
{

    [HttpGet("current/{regionName}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CurrentWeatherDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrent(string regionName)
    {
        var result = await mediator.Send(new GetCurrentWeatherQuery(regionName));
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return Ok(result.Value);
    }
    
    [HttpGet("daily/{regionName}")]
    public async Task<IActionResult> GetDaily(string regionName)
    {
        var result = await mediator.Send(new GetDailyForecastQuery(regionName));
        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }
        return Ok(result.Value); 
    }
}