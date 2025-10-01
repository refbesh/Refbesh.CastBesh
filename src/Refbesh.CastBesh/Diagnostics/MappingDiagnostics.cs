using System.Collections.Concurrent;

namespace Refbesh.CastBesh.Diagnostics;

/// <summary>
/// Collects performance metrics and diagnostics for mappings.
/// </summary>
public static class MappingDiagnostics
{
    private static readonly ConcurrentDictionary<(Type Source, Type Destination), MappingMetrics> _metrics = new();
    private static bool _enabled = false;

    /// <summary>
    /// Enables or disables diagnostics collection.
    /// </summary>
    public static bool Enabled
    {
        get => _enabled;
        set => _enabled = value;
    }

    /// <summary>
    /// Records a mapping operation.
    /// </summary>
    public static void RecordMapping(Type source, Type destination, TimeSpan duration, bool success)
    {
        if (!_enabled) return;

        var key = (source, destination);
        _metrics.AddOrUpdate(key,
            _ => new MappingMetrics
            {
                TotalMappings = 1,
                TotalDuration = duration,
                Failures = success ? 0 : 1,
                LastMappingTime = DateTime.UtcNow
            },
            (_, existing) =>
            {
                lock (existing)
                {
                    existing.TotalMappings++;
                    existing.TotalDuration += duration;
                    if (!success) existing.Failures++;
                    existing.LastMappingTime = DateTime.UtcNow;

                    if (duration < existing.FastestMapping || existing.FastestMapping == TimeSpan.Zero)
                        existing.FastestMapping = duration;

                    if (duration > existing.SlowestMapping)
                        existing.SlowestMapping = duration;
                }
                return existing;
            });
    }

    /// <summary>
    /// Gets metrics for a specific mapping.
    /// </summary>
    public static MappingMetrics? GetMetrics<TSource, TDestination>()
    {
        return GetMetrics(typeof(TSource), typeof(TDestination));
    }

    /// <summary>
    /// Gets metrics for a specific mapping.
    /// </summary>
    public static MappingMetrics? GetMetrics(Type source, Type destination)
    {
        _metrics.TryGetValue((source, destination), out var metrics);
        return metrics;
    }

    /// <summary>
    /// Gets all collected metrics.
    /// </summary>
    public static IReadOnlyDictionary<(Type Source, Type Destination), MappingMetrics> GetAllMetrics()
    {
        return _metrics;
    }

    /// <summary>
    /// Clears all collected metrics.
    /// </summary>
    public static void Clear()
    {
        _metrics.Clear();
    }
}

/// <summary>
/// Performance metrics for a specific mapping pair.
/// </summary>
public sealed class MappingMetrics
{
    public long TotalMappings { get; set; }
    public TimeSpan TotalDuration { get; set; }
    public long Failures { get; set; }
    public TimeSpan FastestMapping { get; set; }
    public TimeSpan SlowestMapping { get; set; }
    public DateTime LastMappingTime { get; set; }

    public TimeSpan AverageDuration =>
        TotalMappings > 0
            ? TimeSpan.FromTicks(TotalDuration.Ticks / TotalMappings)
            : TimeSpan.Zero;

    public double SuccessRate =>
        TotalMappings > 0
            ? (double)(TotalMappings - Failures) / TotalMappings * 100
            : 0;

    public override string ToString()
    {
        return $"Mappings: {TotalMappings}, Avg: {AverageDuration.TotalMilliseconds:F2}ms, " +
               $"Success Rate: {SuccessRate:F1}%, Failures: {Failures}";
    }
}