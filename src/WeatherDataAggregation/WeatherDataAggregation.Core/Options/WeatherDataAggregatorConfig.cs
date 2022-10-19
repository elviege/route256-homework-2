namespace WeatherDataAggregation.Core.Options;

public class WeatherDataAggregatorConfig
{
    public int Interval { get; set; }
    public int GrpcStreamTimeout { get; set; }
}