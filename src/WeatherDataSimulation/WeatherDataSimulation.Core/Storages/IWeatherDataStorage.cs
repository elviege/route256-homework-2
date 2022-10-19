using WeatherDataSimulation.Core.Models;

namespace WeatherDataSimulation.Core.Storages;

public interface IWeatherDataStorage
{
    public void AddData(WeatherDataEvent data); 
    public WeatherDataEvent GetLast(SensorType sensorType);
    public WeatherDataEvent[] GetByInterval(DateTime begin, DateTime end, SensorType sensorType);
}