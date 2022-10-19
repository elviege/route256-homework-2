namespace WeatherDataAggregation.Core.Models;

public struct WeatherModel
{
    public int Temperature { get; set; }
    public int Humidity { get; set; }
    public int CarbonDioxide { get; set; }
}