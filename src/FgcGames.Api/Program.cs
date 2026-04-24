using FgcGames.Api.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.OpenTelemetry();
});

builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

await app.ConfigureAsync();

app.Run();
