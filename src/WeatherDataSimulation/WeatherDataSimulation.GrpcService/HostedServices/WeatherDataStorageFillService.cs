using Microsoft.Extensions.Options;
using WeatherDataSimulation.Core.Models;
using WeatherDataSimulation.Core.Options;
using WeatherDataSimulation.Core.Storages;

namespace WeatherDataSimulation.GrpcService.HostedServices;

public class WeatherDataStorageFillService : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<WeatherDataStorageFillService> _logger;
    private readonly WeatherDataGeneratorConfig _options;
    private readonly IWeatherDataStorage _storage;

    public WeatherDataStorageFillService(IServiceProvider provider, ILogger<WeatherDataStorageFillService> logger, 
        IOptions<WeatherDataGeneratorConfig> options, IWeatherDataStorage storage)
    {
        _provider = provider;
        _logger = logger;
        _options = options.Value;
        _storage = storage;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using var scope = _provider.CreateAsyncScope();
        var interval = _options.GetInterval();
        var rand = new Random();

        for (long i = 1; i < long.MaxValue; i++)
        {
            var mod = i % 2;
            _storage.AddData(new WeatherDataEvent
            {
                Id = i,
                SensorType = (SensorType)mod,
                CreatedDt = DateTime.UtcNow,
                Data = new WeatherModel
                {
                    Temperature = rand.Next(10, 30),
                    Humidity = rand.Next(20, 99),
                    CarbonDioxide = rand.Next(0, 99)
                }
            });
            if (mod == 0)
                await Task.Delay(interval, stoppingToken);
        }
    }
}