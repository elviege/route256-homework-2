using WeatherDataSimulation.GrpcService.GrpcServices;
using WeatherDataSimulation.Core.Options;
using WeatherDataSimulation.Core.Storages;
using WeatherDataSimulation.GrpcService.HostedServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IWeatherDataStorage, WeatherDataStorage>();
builder.Services.AddHostedService<WeatherDataStorageFillService>();
builder.Services.AddGrpc();
builder.Services.Configure<WeatherDataGeneratorConfig>(
    builder.Configuration.GetSection("WeatherDataGeneratorConfig"));
builder.Services.AddMvcCore();

var app = builder.Build();

app.UseRouting();
app.UseEndpoints(b =>
{
    b.MapControllers();
    b.MapGrpcService<WeatherDataGeneratorService>();
});

app.Run();