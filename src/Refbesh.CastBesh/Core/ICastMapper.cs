namespace Refbesh.CastBesh.Core;

/// <summary>
/// Defines the contract for type casting operations between source and destination types.
/// </summary>
/// <typeparam name="TSource">The source type to cast from</typeparam>
/// <typeparam name="TDestination">The destination type to cast to</typeparam>
public interface ICastMapper<TSource, TDestination>
{
    /// <summary>
    /// Maps a source object to a destination object synchronously.
    /// </summary>
    TDestination Map(TSource source);

    /// <summary>
    /// Maps a source object to a destination object asynchronously.
    /// </summary>
    Task<TDestination> MapAsync(TSource source, CancellationToken cancellationToken = default);
}