using Microsoft.AspNetCore.SignalR;
using SmartX.Shared.Models;

namespace SmartX.Api;

// Heavily seeds the system with mock telemetry so you can demo the pipeline under
// load without real ESP32 hardware. PeriodicTimer keeps this fully asynchronous.
public class TelemetrySimulator : BackgroundService
{
    private readonly IHubContext<TelemetryHub> _hub;
    private readonly Random _rng = new();

    private readonly string[] _moistureSensors = { "SOIL-01", "SOIL-02", "SOIL-03" };
    private readonly string[] _powerSensors = { "PWR-01", "PWR-02" };
    private readonly string[] _valveSensors = { "VALVE-01" };

    public TelemetrySimulator(IHubContext<TelemetryHub> hub) => _hub = hub;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(2));
        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            await BroadcastMoisture();
            await BroadcastPower();
            await BroadcastValve();
        }
    }

    private async Task BroadcastMoisture()
    {
        foreach (var sensorId in _moistureSensors)
        {
            float value = _rng.NextDouble() < 0.12
                ? (float)(_rng.Next(0, 2) == 0 ? _rng.Next(0, 5) : _rng.Next(96, 100))
                : (float)Math.Round(35 + _rng.NextDouble() * 15, 1);

            bool anomaly = AnomalyChecker.IsOutOfRange(value, 10f, 90f);
            await _hub.Clients.All.SendAsync("TelemetryReceived", new TelemetryBroadcast(
                "AA:BB:CC:00:00:01", sensorId, "float", value.ToString("F1"), "%", DateTime.UtcNow, anomaly));
        }
    }

    private async Task BroadcastPower()
    {
        foreach (var sensorId in _powerSensors)
        {
            int value = _rng.NextDouble() < 0.10 ? _rng.Next(4000, 5000) : _rng.Next(200, 1500);
            bool anomaly = AnomalyChecker.IsOutOfRange(value, 0, 3000);
            await _hub.Clients.All.SendAsync("TelemetryReceived", new TelemetryBroadcast(
                "AA:BB:CC:00:00:02", sensorId, "int", value.ToString(), "W", DateTime.UtcNow, anomaly));
        }
    }

    private async Task BroadcastValve()
    {
        foreach (var sensorId in _valveSensors)
        {
            bool value = _rng.Next(0, 2) == 0;
            await _hub.Clients.All.SendAsync("TelemetryReceived", new TelemetryBroadcast(
                "AA:BB:CC:00:00:03", sensorId, "bool", value ? "OPEN" : "CLOSED", "", DateTime.UtcNow, false));
        }
    }
}