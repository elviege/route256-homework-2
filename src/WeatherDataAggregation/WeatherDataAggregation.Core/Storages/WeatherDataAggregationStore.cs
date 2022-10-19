using System.Collections.Concurrent;
using WeatherDataAggregation.Core.Models;

namespace WeatherDataAggregation.Core.Storages;

public class WeatherDataAggregationStore : IWeatherDataAggregationStore
{
    private readonly ConcurrentDictionary<long, WeatherDataEvent> _data = new();

    public void AddData(WeatherDataEvent data)
    {
        _data.AddOrUpdate(data.Id, _ => data, (_, _) => data);
    }

    public WeatherDataEvent[] GetAllBySensor(SensorType sensorType)
    {
        return _data.Values.Where(x => x.SensorType == sensorType).ToArray();
    }

    public WeatherDataEvent[] GetData(SensorType sensorType, DateTime begin, DateTime end)
    {
        return _data.Values
            .Where(x => x.SensorType == sensorType && x.CreatedDt >= begin && x.CreatedDt <= end)
            .ToArray();
    }
}