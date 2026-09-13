namespace SmartX.Shared.Models;

public static class TelemetryBatchProcessor
{
    // Each sensor reports a different reading count per cycle, so a jagged array
    // (not a rectangular 2D array) is the correct structure for the raw input.
    public static List<List<double>> FlattenJaggedBatch(double[][] rawSensorBatches)
    {
        var optimised = new List<List<double>>(rawSensorBatches.Length);
        foreach (double[] sensorRow in rawSensorBatches)
        {
            optimised.Add(new List<double>(sensorRow)); // raw array -> optimised List<T>
        }
        return optimised;
    }

    public static double[][] BuildSeedJaggedBatch() => new double[][]
    {
        new double[] { 42.1, 42.3, 41.9 },
        new double[] { 38.0, 37.8, 38.2, 37.9 },
        new double[] { 55.5 },
        new double[] { 12.0, 96.4, 45.1, 44.9, 45.6 } // includes an anomalous spike
    };

    public static double Average(List<double> readings) =>
        readings.Count == 0 ? 0 : readings.Sum() / readings.Count;
}