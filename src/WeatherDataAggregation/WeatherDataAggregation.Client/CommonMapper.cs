using Google.Protobuf.WellKnownTypes;
using WeatherDataAggregation.Core.Models;
using WeatherDataSimulation.GrpcService;

namespace WeatherDataAggregation.Client;

public static class CommonMapper
{
    public static WeatherDataEvent MapToWeatherDataEvent(WeatherReply d)
    {
        if (d is null) return null;
        return new WeatherDataEvent
        {
            Id = d.Id,
            CreatedDt = d.CreatedAt.ToDateTime(),
            Data = new WeatherModel
            {
                Temperature = d.Data.Temperature,
                Humidity = d.Data.Humidity,
                CarbonDioxide = d.Data.CarbonDioxide
            },
            SensorType = d.Sensor switch
            {
                sensor.Inside => SensorType.Inside,
                sensor.Outside => SensorType.Outside
            }
        };
    }
}