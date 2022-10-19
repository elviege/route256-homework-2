using WeatherDataAggregation.Client.HostedServices;
using WeatherDataAggregation.Core.Options;
using WeatherDataAggregation.Core.Storages;
using WeatherDataSimulation.GrpcService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<WeatherDataAggregatorConfig>(
    builder.Configuration.GetSection("WeatherDataAggregatorConfig"));
builder.Services.AddSingleton<IWeatherDataAggregationStore, WeatherDataAggregationStore>();
builder.Services.AddSingleton<IWeatherDataSubscribeStore, WeatherDataSubscribeStore>();
builder.Services.AddHostedService<WeatherDataAggregationService>();
builder.Services.AddGrpcClient<WheatherDataGenerator.WheatherDataGeneratorClient>(
    option =>
    {
        option.Address = new Uri("http://localhost:5278");
    });

builder.Services.AddMvcCore();

var app = builder.Build();
app.UseRouting();
app.UseEndpoints(b =>
{
    b.MapControllers();
});

app.Run();