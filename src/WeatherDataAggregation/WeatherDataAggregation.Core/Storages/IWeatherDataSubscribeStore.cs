using WeatherDataAggregation.Core.Models;

namespace WeatherDataAggregation.Core.Storages;

public interface IWeatherDataSubscribeStore
{
    public void Subscribe(SensorType[] sensorTypes);
    public void Unsubscribe(SensorType[] sensorTypes);
    public bool IsSubscribed(SensorType sensorType);
}