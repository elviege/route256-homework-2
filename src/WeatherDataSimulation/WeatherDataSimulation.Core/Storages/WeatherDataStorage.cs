using System.Collections.Concurrent;
using WeatherDataSimulation.Core.Models;

namespace WeatherDataSimulation.Core.Storages;

public class WeatherDataStorage : IWeatherDataStorage
{
    private readonly ConcurrentDictionary<long, WeatherDataEvent> _data = new();

    public void AddData(WeatherDataEvent data)
    {
        _data.AddOrUpdate(data.Id, _ => data, (_, _) => data);
    }

    public WeatherDataEvent GetLast(SensorType sensorType)
    {
        if (_data.IsEmpty) return null;
        return _data.Values.Last(x => x.SensorType == sensorType);
    }

    public WeatherDataEvent[] GetByInterval(DateTime begin, DateTime end, SensorType sensorType)
    {
        return _data.Values.Where(x => x.SensorType == sensorType && x.CreatedDt >= begin && x.CreatedDt >= end).ToArray();
    }
}