namespace WeatherDataSimulation.Core.Options;

public class WeatherDataGeneratorConfig
{
    public int Interval { get; set; }
    
    public int GetInterval()
    {
        return Interval switch
        {
            < 200 => 200,
            > 2000 => 2000,
            _ => Interval
        };
    }
}