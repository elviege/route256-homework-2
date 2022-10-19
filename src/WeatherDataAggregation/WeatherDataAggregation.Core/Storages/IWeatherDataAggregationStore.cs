using WeatherDataAggregation.Core.Models;

namespace WeatherDataAggregation.Core.Storages;

public interface IWeatherDataAggregationStore
{
    public void AddData(WeatherDataEvent data);
    public WeatherDataEvent[] GetAllBySensor(SensorType sensorType);
    public WeatherDataEvent[] GetData(SensorType sensorType, DateTime begin, DateTime end);
}