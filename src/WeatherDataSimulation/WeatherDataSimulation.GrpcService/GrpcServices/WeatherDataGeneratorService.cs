using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Options;
using WeatherDataSimulation.Core.Models;
using WeatherDataSimulation.Core.Options;
using WeatherDataSimulation.Core.Storages;

namespace WeatherDataSimulation.GrpcService.GrpcServices;

public class WeatherDataGeneratorService : WheatherDataGenerator.WheatherDataGeneratorBase
{
    private readonly ILogger<WeatherDataGeneratorService> _logger;
    private readonly IWeatherDataStorage _storage;

    private readonly int _interval;

    public WeatherDataGeneratorService(ILogger<WeatherDataGeneratorService> logger, 
        IOptions<WeatherDataGeneratorConfig> options, IWeatherDataStorage storage)
    {
        _logger = logger;
        _storage = storage;

        _interval = options.Value.GetInterval();
    }

    public override Task<WeatherReply> GetLast(WeatherRequest request, ServerCallContext context)
    {
        var sensorType = (SensorType)((int)request.Sensor);
        var d = _storage.GetLast(sensorType);
        return Task.FromResult(CommonMapper.MapToWeatherReply(d));
    }

    public override async Task GetDataStream(WeatherRequest request, IServerStreamWriter<WeatherReply> responseStream, ServerCallContext context)
    {
        try
        {
            long prevId = 0;
            while (!context.CancellationToken.IsCancellationRequested)
            {
                var sensorType = (SensorType)((int)request.Sensor);
                var data = _storage.GetLast(sensorType);
                if (data is null || prevId == data.Id)
                {
                    await Task.Delay(_interval, context.CancellationToken);
                    continue;
                }

                prevId = data.Id;
                var result = CommonMapper.MapToWeatherReply(data);
                await responseStream.WriteAsync(result);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("GetDataStream operation was canceled");
        }
    }

    public override async Task GetDataBiStream(
        IAsyncStreamReader<WeatherRequest> requestStream, 
        IServerStreamWriter<WeatherReply> responseStream,
        ServerCallContext context)
    {
        try
        {
            while (await requestStream.MoveNext() && !context.CancellationToken.IsCancellationRequested)
            {
                var message = requestStream.Current;
                var sensorType = (SensorType)((int)message.Sensor);
                var data = _storage.GetLast(sensorType);
                var result = CommonMapper.MapToWeatherReply(data);
                await responseStream.WriteAsync(result);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("GetDataBiStream operation was canceled");
        }
    }
}