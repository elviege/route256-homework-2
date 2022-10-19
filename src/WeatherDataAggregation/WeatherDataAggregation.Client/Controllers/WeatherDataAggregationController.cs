using Microsoft.AspNetCore.Mvc;
using WeatherDataAggregation.Core.Models;
using WeatherDataAggregation.Core.Storages;

namespace WeatherDataAggregation.Client.Controllers;

[Route("aggregator")]
public class WeatherDataAggregationController : ControllerBase
{
    private readonly IWeatherDataAggregationStore _dataStorage;
    private readonly IWeatherDataSubscribeStore _subscribeStorage;
    
    public WeatherDataAggregationController(IWeatherDataAggregationStore dataStorage, IWeatherDataSubscribeStore subscribeStorage)
    {
        _dataStorage = dataStorage;
        _subscribeStorage = subscribeStorage;
    }

    [HttpGet("data/{sensorType}")]
    public async Task<ActionResult<WeatherDataEvent[]>> GetAllData(SensorType sensorType)
    {
        var data = _dataStorage.GetAllBySensor(sensorType);
        return Ok(data);
    }

    [HttpGet("AverageTemperature/{sensorType}/{begin}/{end}")]
    public async Task<ActionResult<int?>> GetAverageTemperature(SensorType sensorType, DateTime begin, DateTime end)
    {
        var data = _dataStorage.GetData(sensorType, begin, end)
            .Select(x => x.Data.Temperature).ToArray();
        if (data.Length == 0) return Ok(null);
        
        var result = data.Sum() / data.Length;
        return Ok(result);
    }
    
    [HttpGet("AverageHumidity/{sensorType}/{begin}/{end}")]
    public async Task<ActionResult<int?>> GetAverageHumidity(SensorType sensorType, DateTime begin, DateTime end)
    {
        var data = _dataStorage.GetData(sensorType, begin, end)
            .Select(x => x.Data.Humidity).ToArray();
        if (data.Length == 0) return Ok(null);
        
        var result = data.Sum() / data.Length;
        return Ok(result);
    }
    
    [HttpGet("AverageCarbonDioxide/{sensorType}/{begin}/{end}")]
    public async Task<ActionResult<int?>> GetAverageCarbonDioxide(SensorType sensorType, DateTime begin, DateTime end)
    {
        var data = _dataStorage.GetData(sensorType, begin, end)
            .Select(x => x.Data.CarbonDioxide).ToArray();
        if (data.Length == 0) return Ok(null);
        
        var result = data.Sum() / data.Length;
        return Ok(result);
    }

    [HttpPost("subscribe")]
    public async Task<ActionResult> Subscribe(SubscribeRequestModel model)
    {
        //TODO: validate incoming model?
        _subscribeStorage.Subscribe(model.SensorTypes);
        
        return Ok("subscribe success!");
    }
    
    [HttpPost("unsubscribe")]
    public async Task<ActionResult> Unsubscribe(SubscribeRequestModel model)
    {
        //TODO: validate incoming model?
        _subscribeStorage.Unsubscribe(model.SensorTypes);
        
        return Ok("unsubscribe success!");
    }
}