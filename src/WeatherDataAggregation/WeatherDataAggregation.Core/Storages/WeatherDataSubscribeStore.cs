using System.Collections.Concurrent;
using WeatherDataAggregation.Core.Models;

namespace WeatherDataAggregation.Core.Storages;

public class WeatherDataSubscribeStore : IWeatherDataSubscribeStore
{
    private readonly ConcurrentDictionary<SensorType, bool> _data = new();
    
    public void Subscribe(SensorType[] sensorTypes)
    {
        foreach (var sensorType in sensorTypes)
        {
            _data.AddOrUpdate(sensorType, _ => true, (_, _) => true);
        }
    }

    public void Unsubscribe(SensorType[] sensorTypes)
    {
        foreach (var sensorType in sensorTypes)
        {
            _data.AddOrUpdate(sensorType, _ => false, (_, _) => false);
        }
    }

    public bool IsSubscribed(SensorType sensorType)
    {
        if (_data.TryGetValue(sensorType, out bool flag))
        {
            return flag;
        }

        return false;
    }
}