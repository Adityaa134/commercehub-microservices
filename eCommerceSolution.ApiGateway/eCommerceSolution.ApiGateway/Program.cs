using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

builder.Services
    .AddOcelot(builder.Configuration)
    .AddPolly();

var app = builder.Build();

// Uses caller's IP as ClientId so Ocelot rate limits each user individually instead of sharing one quota.
app.Use(async (context, next) =>
{
    if (!context.Request.Headers.ContainsKey("ClientId"))
    {
        var clientIp = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                       ?? context.Connection.RemoteIpAddress?.ToString()
                       ?? "unknown-client";
        context.Request.Headers["ClientId"] = clientIp;
    }
    await next();
});

await app.UseOcelot();

app.Run();
