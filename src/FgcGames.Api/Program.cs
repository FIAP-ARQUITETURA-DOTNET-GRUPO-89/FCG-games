using FgcGames.Api.Extensions;
using Serilog;

// Permite DateTime sem Kind (Unspecified) em colunas timestamp with time zone do PostgreSQL
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

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

app.MapDefaultEndpoints();
await app.Configure();

app.Run();
