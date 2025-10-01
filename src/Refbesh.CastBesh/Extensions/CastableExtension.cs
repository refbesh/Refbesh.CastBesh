using Refbesh.CastBesh.Casting;
using System.Runtime.CompilerServices;

namespace Refbesh.CastBesh.Extensions;

/// <summary>
/// Extension methods for fluent casting syntax.
/// </summary>
public static class CastableExtensions
{
    /// <summary>
    /// Casts source object to destination type using registered mapper.
    /// Usage: var dto = entity.CastTo&lt;UserEntity, UserDto&gt;();
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TDestination CastTo<TSource, TDestination>(this TSource source)
    {
        return CastEngine.Cast<TSource, TDestination>(source);
    }

    /// <summary>
    /// Attempts to cast, returning default on failure.
    /// </summary>
    public static bool TryCastTo<TSource, TDestination>(
        this TSource source,
        out TDestination? result)
    {
        return CastEngine.TryCast(source, out result);
    }

    /// <summary>
    /// Casts source object to destination type asynchronously.
    /// </summary>
    public static Task<TDestination> CastToAsync<TSource, TDestination>(
        this TSource source,
        CancellationToken cancellationToken = default)
    {
        return CastEngine.CastAsync<TSource, TDestination>(source, cancellationToken);
    }
}
