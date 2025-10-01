using Refbesh.CastBesh.Registry;
using System.Runtime.CompilerServices;

namespace Refbesh.CastBesh.Casting;

/// <summary>
/// Wrapper that enables automatic cast syntax without modifying user's entities.
/// Usage: var dto = userEntity.ToCastable().To&lt;UserDto&gt;();
/// Or shorter: var dto = userEntity.As&lt;UserDto&gt;();
/// </summary>
public readonly struct CastableWrapper<TSource>
{
    private readonly TSource _value;

    internal CastableWrapper(TSource value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Casts to the destination type using registered mapper.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TDestination To<TDestination>()
    {
        var mapper = CastMapperRegistry.Instance.GetCompiledMapper<TSource, TDestination>();
        return mapper(_value);
    }

    /// <summary>
    /// Casts to the destination type asynchronously.
    /// </summary>
    public Task<TDestination> ToAsync<TDestination>(CancellationToken cancellationToken = default)
    {
        return CastEngine.CastAsync<TSource, TDestination>(_value, cancellationToken);
    }

    /// <summary>
    /// Attempts to cast, returning success status.
    /// </summary>
    public bool TryTo<TDestination>(out TDestination? result)
    {
        return CastEngine.TryCast(_value, out result);
    }

    /// <summary>
    /// Gets the original value.
    /// </summary>
    public TSource Value => _value;

    /// <summary>
    /// Implicit conversion to allow: UserDto dto = userEntity.ToCastable();
    /// when the target type can be inferred.
    /// </summary>
    public static implicit operator TSource(CastableWrapper<TSource> wrapper) => wrapper._value;
}

/// <summary>
/// Extension methods to enable automatic casting without modifying user entities.
/// </summary>
public static class AutoCastExtensions
{
    /// <summary>
    /// Wraps the source object for casting.
    /// Usage: var dto = userEntity.ToCastable().To&lt;UserDto&gt;();
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static CastableWrapper<TSource> ToCastable<TSource>(this TSource source)
    {
        return new CastableWrapper<TSource>(source);
    }

    /// <summary>
    /// Direct cast syntax - cleaner API.
    /// Usage: var dto = userEntity.As&lt;UserDto&gt;();
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TDestination As<TDestination>(this object source)
    {
        var sourceType = source.GetType();
        var mapper = CastMapperRegistry.Instance.GetCompiledMapper(sourceType, typeof(TDestination));
        return (TDestination)mapper(source);
    }

    /// <summary>
    /// Strongly-typed cast syntax for better performance.
    /// Usage: var dto = userEntity.As&lt;UserEntity, UserDto&gt;();
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static TDestination As<TSource, TDestination>(this TSource source)
    {
        return CastEngine.Cast<TSource, TDestination>(source);
    }

    /// <summary>
    /// Async cast syntax.
    /// Usage: var dto = await userEntity.AsAsync&lt;UserDto&gt;();
    /// </summary>
    public static Task<TDestination> AsAsync<TDestination>(
        this object source,
        CancellationToken cancellationToken = default)
    {
        var sourceType = source.GetType();

        // Use reflection to call the generic CastAsync method
        var method = typeof(CastEngine)
            .GetMethod(nameof(CastEngine.CastAsync))
            ?.MakeGenericMethod(sourceType, typeof(TDestination));

        return (Task<TDestination>)method!.Invoke(null, new[] { source, cancellationToken })!;
    }

    /// <summary>
    /// Strongly-typed async cast for better performance.
    /// Usage: var dto = await userEntity.AsAsync&lt;UserEntity, UserDto&gt;();
    /// </summary>
    public static Task<TDestination> AsAsync<TSource, TDestination>(
        this TSource source,
        CancellationToken cancellationToken = default)
    {
        return CastEngine.CastAsync<TSource, TDestination>(source, cancellationToken);
    }

    /// <summary>
    /// Safe casting that returns default on failure.
    /// Usage: if (userEntity.TryAs&lt;UserDto&gt;(out var dto)) { ... }
    /// </summary>
    public static bool TryAs<TDestination>(
        this object source,
        out TDestination? result)
    {
        try
        {
            result = source.As<TDestination>();
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }

    /// <summary>
    /// Strongly-typed safe casting.
    /// Usage: if (userEntity.TryAs&lt;UserEntity, UserDto&gt;(out var dto)) { ... }
    /// </summary>
    public static bool TryAs<TSource, TDestination>(
        this TSource source,
        out TDestination? result)
    {
        return CastEngine.TryCast(source, out result);
    }
}