using Microsoft.AspNetCore.SignalR;
using SmartX.Api;
using SmartX.Shared.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSignalR();
builder.Services.AddHostedService<TelemetrySimulator>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("SmartXClient", policy =>
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();
app.UseCors("SmartXClient");
app.MapHub<TelemetryHub>("/hubs/telemetry");

var sensorRegistry = new List<SensorRegistration>();
var registryLock = new object();

// Sensor registration
app.MapPost("/api/sensors", (SensorRegistration registration) =>
{
    lock (registryLock) { sensorRegistry.Add(registration); }
    return Results.Created($"/api/sensors/{registration.MacAddress}", registration);
});

app.MapGet("/api/sensors", () =>
{
    lock (registryLock) { return Results.Ok(sensorRegistry.ToList()); }
});

// Telemetry ingestion — one endpoint per concrete type, same generic packet underneath
app.MapPost("/api/telemetry/float", async (TelemetryPacket<float> packet, IHubContext<TelemetryHub> hub) =>
{
    bool anomaly = AnomalyChecker.IsOutOfRange(packet.Value, 10f, 90f);
    await hub.Clients.All.SendAsync("TelemetryReceived", new TelemetryBroadcast(
        packet.DeviceMac, packet.SensorId, "float", packet.Value.ToString("F1"), packet.Unit, packet.Timestamp, anomaly));
    return Results.Ok(new { received = true, anomaly });
});

app.MapPost("/api/telemetry/int", async (TelemetryPacket<int> packet, IHubContext<TelemetryHub> hub) =>
{
    bool anomaly = AnomalyChecker.IsOutOfRange(packet.Value, 0, 3000);
    await hub.Clients.All.SendAsync("TelemetryReceived", new TelemetryBroadcast(
        packet.DeviceMac, packet.SensorId, "int", packet.Value.ToString(), packet.Unit, packet.Timestamp, anomaly));
    return Results.Ok(new { received = true, anomaly });
});

app.MapPost("/api/telemetry/bool", async (TelemetryPacket<bool> packet, IHubContext<TelemetryHub> hub) =>
{
    await hub.Clients.All.SendAsync("TelemetryReceived", new TelemetryBroadcast(
        packet.DeviceMac, packet.SensorId, "bool", packet.Value ? "OPEN" : "CLOSED", packet.Unit, packet.Timestamp, false));
    return Results.Ok(new { received = true, anomaly = false });
});

// File upload — configs, deployment photos, hardware logs
app.MapPost("/api/sensors/{mac}/attachment", async (string mac, IFormFile file) =>
{
    if (file.Length == 0) return Results.BadRequest("Empty file.");

    var uploadsDir = Path.Combine(AppContext.BaseDirectory, "uploads", mac.Replace(":", "-"));
    Directory.CreateDirectory(uploadsDir);
    var safeFileName = Path.GetFileName(file.FileName);
    var destinationPath = Path.Combine(uploadsDir, safeFileName);

    await using (var stream = File.Create(destinationPath))
    {
        await file.CopyToAsync(stream);
    }

    lock (registryLock)
    {
        sensorRegistry.FirstOrDefault(s => s.MacAddress == mac)?.Attachments.Add(safeFileName);
    }
    return Results.Ok(new { fileName = safeFileName, size = file.Length });
}).DisableAntiforgery();

// Recursion demo
app.MapGet("/api/deployment/validate", () =>
{
    var tree = DeploymentValidator.BuildSeedTree();
    bool isValid = DeploymentValidator.IsValidDeployment(tree);
    return Results.Ok(new { isValid, firstInvalidPath = isValid ? null : DeploymentValidator.FindFirstInvalidPath(tree) });
});

// Jagged array -> List<T> demo
app.MapGet("/api/telemetry/history/soil-batch", () =>
{
    var optimised = TelemetryBatchProcessor.FlattenJaggedBatch(TelemetryBatchProcessor.BuildSeedJaggedBatch());
    var summary = optimised.Select((readings, index) => new
    {
        sensor = $"SOIL-{index + 1:00}",
        readings,
        average = Math.Round(TelemetryBatchProcessor.Average(readings), 2)
    });
    return Results.Ok(summary);
});

// Operator overloading demo
app.MapGet("/api/power/aggregate", () =>
{
    var meter1 = new PowerReading("Meter1", 850);
    var meter2 = new PowerReading("Meter2", 620);
    var aggregate = meter1 + meter2;
    double delta = meter1 - meter2;
    bool meter1IsHigher = meter1 > meter2;

    return Results.Ok(new
    {
        meter1 = meter1.ToString(),
        meter2 = meter2.ToString(),
        aggregate = aggregate.ToString(),
        delta,
        meter1IsHigher
    });
});

app.Run();