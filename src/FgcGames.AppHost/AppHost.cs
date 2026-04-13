var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("Postgres", port: 5432)
                .WithLifetime(ContainerLifetime.Persistent)
                .WithPgAdmin(c => c.WithLifetime(ContainerLifetime.Persistent))
                .AddDatabase("Default", "fgcgames-db");

builder.AddProject<Projects.FgcGames_Api>("fgcgames-api")
        .WithReference(postgres)
        .WaitFor(postgres);

builder.Build().Run();
