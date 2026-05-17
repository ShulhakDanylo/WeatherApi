using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Weather.BLL.Helpers; 

namespace WeatherApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseApiController : ControllerBase
{
    private IMediator? _mediator;

    protected IMediator Mediator => _mediator ??=
        HttpContext.RequestServices.GetService<IMediator>()!;

    protected ActionResult HandleResult<T>(Result<T> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        var problemsFactory = HttpContext.RequestServices
            .GetRequiredService<ProblemDetailsFactory>();

        
        if (result.Error!.Contains("not found", StringComparison.OrdinalIgnoreCase))
        {
            return NotFound(problemsFactory.CreateProblemDetails(HttpContext, 
                statusCode: StatusCodes.Status404NotFound, detail: result.Error));
        }
        
        if (result.Error!.Equals("Unauthorized", StringComparison.OrdinalIgnoreCase))
        {
            return Unauthorized(problemsFactory.CreateProblemDetails(HttpContext, 
                statusCode: StatusCodes.Status401Unauthorized));
        }
        
        return BadRequest(problemsFactory.CreateProblemDetails(
            HttpContext,
            statusCode: StatusCodes.Status400BadRequest,
            detail: result.Error));
    }
}