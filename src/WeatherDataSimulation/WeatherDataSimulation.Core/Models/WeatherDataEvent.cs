namespace WeatherDataSimulation.Core.Models;

public class WeatherDataEvent
{
    public long Id { get; set; }
    public SensorType SensorType { get; set; }
    public WeatherModel Data { get; set; }
    public DateTime CreatedDt { get; set; }
}