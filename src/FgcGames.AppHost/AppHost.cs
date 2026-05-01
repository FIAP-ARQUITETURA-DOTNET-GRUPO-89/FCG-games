using Microsoft.Extensions.Hosting;

// Permite transporte HTTP (necessário ao rodar sem HTTPS no dashboard do Aspire)
Environment.SetEnvironmentVariable("ASPIRE_ALLOW_UNSECURED_TRANSPORT", "true");

// Fallback para quando o projeto é rodado sem o launch profile (VS sem extensão Aspire, debug direto, etc.)
if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_URLS")))
    Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "http://localhost:15245");

if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL")) &&
    string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPIRE_DASHBOARD_OTLP_HTTP_ENDPOINT_URL")))
    Environment.SetEnvironmentVariable("ASPIRE_DASHBOARD_OTLP_HTTP_ENDPOINT_URL", "http://localhost:19262");

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.Environment.IsDevelopment()
    ? builder.AddPostgres("Postgres")
        .WithLifetime(ContainerLifetime.Session)
        .AddDatabase("Default", "fgcgames-db")
    : builder.AddPostgres("Postgres", port: 5432)
        .WithLifetime(ContainerLifetime.Persistent)
        .WithPgAdmin(c => c.WithLifetime(ContainerLifetime.Persistent))
        .AddDatabase("Default", "fgcgames-db");

builder.AddProject<Projects.FgcGames_Api>("fgcgames-api")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", builder.Environment.EnvironmentName)
    .WithReference(postgres)
    .WaitFor(postgres);

builder.Build().Run();
