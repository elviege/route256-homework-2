using System.Diagnostics;
using Google.Protobuf.WellKnownTypes;
using WeatherDataSimulation.Core.Models;

namespace WeatherDataSimulation.GrpcService;

public static class CommonMapper
{
    public static WeatherReply MapToWeatherReply(WeatherDataEvent d)
    {
        if (d is null) return null;
        return new WeatherReply
        {
            Id = d.Id,
            Data =  new WeatherData
            {
                Temperature = d.Data.Temperature,
                Humidity = d.Data.Humidity,
                CarbonDioxide = d.Data.CarbonDioxide
            },
            CreatedAt = d.CreatedDt.ToTimestamp(),
            Sensor = d.SensorType switch
            {
                SensorType.Inside => sensor.Inside,
                SensorType.Outside => sensor.Outside
            }
        };
    }
}