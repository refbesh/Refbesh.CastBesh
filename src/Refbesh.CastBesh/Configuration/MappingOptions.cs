namespace Refbesh.CastBesh.Configuration;

/// <summary>
/// Defines how null values should be handled during mapping.
/// </summary>
public enum NullHandling
{
    /// <summary>Throw exception when source is null</summary>
    ThrowException,
    /// <summary>Return default(TDestination) when source is null</summary>
    ReturnDefault,
    /// <summary>Return null when source is null (for reference types)</summary>
    ReturnNull
}

/// <summary>
/// Global configuration options for CastBesh mapping behavior.
/// </summary>
public sealed class MappingOptions
{
    /// <summary>
    /// How to handle null source values. Default: ThrowException
    /// </summary>
    public NullHandling NullHandling { get; set; } = NullHandling.ThrowException;

    /// <summary>
    /// Whether to validate configuration at startup. Default: true
    /// </summary>
    public bool ValidateConfiguration { get; set; } = true;

    /// <summary>
    /// Whether to cache compiled mappers. Default: true
    /// </summary>
    public bool EnableCaching { get; set; } = true;

    /// <summary>
    /// Whether to collect performance metrics. Default: false
    /// </summary>
    public bool EnableDiagnostics { get; set; } = false;
}