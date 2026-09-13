namespace SmartX.Shared.Models;

// Generic wrapper — one class handles float, int and bool payloads with no boxing,
// because T is bound at compile time per instance instead of stored as `object`.
public class TelemetryPacket<T>
{
    public string DeviceMac { get; set; } = string.Empty;
    public string SensorId { get; set; } = string.Empty;
    public T Value { get; set; } = default!;
    public string Unit { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public TelemetryPacket() { }

    public TelemetryPacket(string deviceMac, string sensorId, T value, string unit)
    {
        DeviceMac = deviceMac;
        SensorId = sensorId;
        Value = value;
        Unit = unit;
    }

    public override string ToString() => $"[{Timestamp:HH:mm:ss}] {SensorId} ({DeviceMac}) = {Value}{Unit}";
}