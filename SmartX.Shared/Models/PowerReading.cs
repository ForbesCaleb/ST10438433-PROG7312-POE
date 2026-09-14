namespace SmartX.Shared.Models;

public class PowerReading
{
    public string MeterId { get; set; }
    public double Watts { get; set; }

    public PowerReading(string meterId, double watts)
    {
        MeterId = meterId;
        Watts = watts;
    }

    // Meter3 = Meter1 + Meter2  (aggregate load)
    public static PowerReading operator +(PowerReading a, PowerReading b)
        => new($"{a.MeterId}+{b.MeterId}", a.Watts + b.Watts);

    // Delta comparison between two readings
    public static double operator -(PowerReading a, PowerReading b) => a.Watts - b.Watts;

    // C# requires > and < to be defined as a pair
    public static bool operator >(PowerReading a, PowerReading b) => a.Watts > b.Watts;
    public static bool operator <(PowerReading a, PowerReading b) => a.Watts < b.Watts;

    public string Describe() => $"{MeterId} reading: {Watts:F1} W";
    public string Describe(string unitLabel) => $"{MeterId} reading: {Watts:F1} {unitLabel}";
    public string Describe(bool includeTimestamp, DateTime timestamp) =>
        includeTimestamp ? $"{MeterId} reading: {Watts:F1} W (at {timestamp:T})" : Describe();

    public override string ToString() => $"{MeterId}: {Watts:F1} W";
}