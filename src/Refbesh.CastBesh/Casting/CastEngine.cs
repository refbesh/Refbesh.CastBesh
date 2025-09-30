// Casting/Castable.cs
using Refbesh.CastBesh.Registry;

namespace Refbesh.CastBesh.Casting;

/// <summary>
/// Core casting engine that powers the explicit cast operators.
/// This class is used internally by wrapper types with cast operators.
/// </summary>
public static class CastEngine
{
    /// <summary>
    /// Performs a synchronous cast from source to destination type.
    /// </summary>
    public static TDestination Cast<TSource, TDestination>(TSource source)
    {
        var mapper = CastMapperRegistry.Instance.Get<TSource, TDestination>();
        return mapper.Map(source);
    }

    /// <summary>
    /// Performs an asynchronous cast from source to destination type.
    /// </summary>
    public static Task<TDestination> CastAsync<TSource, TDestination>(
        TSource source,
        CancellationToken cancellationToken = default)
    {
        var mapper = CastMapperRegistry.Instance.Get<TSource, TDestination>();
        return mapper.MapAsync(source, cancellationToken);
    }

    /// <summary>
    /// Attempts to cast, returning default on failure.
    /// </summary>
    public static bool TryCast<TSource, TDestination>(
        TSource source,
        out TDestination? result)
    {
        try
        {
            result = Cast<TSource, TDestination>(source);
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }
}