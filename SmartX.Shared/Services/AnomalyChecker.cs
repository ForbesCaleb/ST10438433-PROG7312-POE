namespace SmartX.Shared.Models;

public static class AnomalyChecker
{
    public static bool IsOutOfRange<T>(T value, T min, T max) where T : IComparable<T>
        => value.CompareTo(min) < 0 || value.CompareTo(max) > 0;
}