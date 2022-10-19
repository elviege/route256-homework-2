using Grpc.Core;
using Microsoft.Extensions.Options;
using Polly;
using WeatherDataAggregation.Core.Models;
using WeatherDataAggregation.Core.Options;
using WeatherDataAggregation.Core.Storages;
using WeatherDataSimulation.GrpcService;

namespace WeatherDataAggregation.Client.HostedServices;

public class WeatherDataAggregationService : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly IWeatherDataAggregationStore _dataStorage;
    private readonly IWeatherDataSubscribeStore _subscribeStorage;
    private readonly WeatherDataAggregatorConfig _options;
    private readonly ILogger<WeatherDataAggregationService> _logger;

    public WeatherDataAggregationService(IServiceProvider provider, 
        IWeatherDataAggregationStore dataStorage, IWeatherDataSubscribeStore subscribeStorage,
        IOptions<WeatherDataAggregatorConfig> options, ILogger<WeatherDataAggregationService> logger)
    {
        _provider = provider;
        _dataStorage = dataStorage;
        _subscribeStorage = subscribeStorage;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _subscribeStorage.Subscribe(new [] { SensorType.Inside, SensorType.Outside });
        
        await using var scope = _provider.CreateAsyncScope();
        var client = scope.ServiceProvider.GetRequiredService<WheatherDataGenerator.WheatherDataGeneratorClient>();
        
        var stream = client.GetDataBiStream();
        var cts = new CancellationTokenSource(_options.GrpcStreamTimeout);
        
        var policy = Policy
            .Handle<Exception>()
            .WaitAndRetryForeverAsync(
                attempt => TimeSpan.FromSeconds(15),
                (exception, timespan) =>
                {
                    _logger.LogError(exception, "Error handling gRPC stream. Reconnecting...");
                    stream = client.GetDataBiStream();
                    cts = new CancellationTokenSource(timespan.Add(TimeSpan.FromMilliseconds(_options.GrpcStreamTimeout)));
                });

        await policy.ExecuteAsync(async () =>
        {
            var response = Task.Run(async () =>
            {
                long prevId = 0;
                while (await stream.ResponseStream.MoveNext(cts.Token))
                {
                    var msg = stream.ResponseStream.Current;
                    if (msg is null) continue;
                    if (msg.Id == prevId) continue;
                
                    var data = CommonMapper.MapToWeatherDataEvent(msg);
                    _dataStorage.AddData(data);
                    prevId = msg.Id;

                    await Task.Delay(_options.Interval, cts.Token);
                }
            }, stoppingToken);
        
            while (!cts.Token.IsCancellationRequested)
            {
                if (_subscribeStorage.IsSubscribed(SensorType.Inside))
                {
                    await stream.RequestStream.WriteAsync(new WeatherRequest
                    {
                        Sensor = sensor.Inside
                    }, cts.Token); 
                }
                if (_subscribeStorage.IsSubscribed(SensorType.Outside))
                {
                    await stream.RequestStream.WriteAsync(new WeatherRequest
                    {
                        Sensor = sensor.Outside
                    }, cts.Token); 
                }
            }
        });
    }
}