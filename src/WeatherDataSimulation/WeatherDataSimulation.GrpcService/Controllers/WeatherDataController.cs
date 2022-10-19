using Microsoft.AspNetCore.Mvc;
using WeatherDataSimulation.Core.Models;
using WeatherDataSimulation.Core.Storages;

namespace WeatherDataSimulation.GrpcService.Controllers;

[Route("weather")]
public class WeatherDataController : ControllerBase
{
    private readonly IWeatherDataStorage _storage;

    public WeatherDataController(IWeatherDataStorage storage)
    {
        _storage = storage;
    }

    [HttpGet("lastdata")]
    public async Task<ActionResult<WeatherDataEvent[]>> GetLastData()
    {
        var result = new[] 
        {
            _storage.GetLast(SensorType.Inside),
            _storage.GetLast(SensorType.Outside)
        }
            .ToArray();

        return Ok(result);
    }
}