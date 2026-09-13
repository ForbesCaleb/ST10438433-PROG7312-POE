namespace SmartX.Shared.Models;

// Flat shape pushed over SignalR — TelemetryPacket<T> is generic at compile time,
// but a live broadcast needs one concrete shape at runtime, so the API converts to this.
public record TelemetryBroadcast(
    string DeviceMac, string SensorId, string ValueType,
    string DisplayValue, string Unit, DateTime Timestamp, bool IsAnomaly);