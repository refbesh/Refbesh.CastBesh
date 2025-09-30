using System.Collections.Concurrent;
using Refbesh.CastBesh.Core;
using Refbesh.CastBesh.Core.Exceptions;

namespace Refbesh.CastBesh.Registry;

/// <summary>
/// Thread-safe registry for storing and retrieving cast mappers.
/// Implements Singleton pattern for global access.
/// </summary>
public sealed class CastMapperRegistry
{
    private static readonly Lazy<CastMapperRegistry> _instance =
        new(() => new CastMapperRegistry(), LazyThreadSafetyMode.ExecutionAndPublication);

    private readonly ConcurrentDictionary<(Type Source, Type Destination), object> _mappers;
    private readonly ConcurrentDictionary<(Type Source, Type Destination), Func<object, object>> _compiledMappers;

    public static CastMapperRegistry Instance => _instance.Value;

    private CastMapperRegistry()
    {
        _mappers = new ConcurrentDictionary<(Type, Type), object>();
        _compiledMappers = new ConcurrentDictionary<(Type, Type), Func<object, object>>();
    }

    /// <summary>
    /// Registers a mapper for the specified type pair.
    /// </summary>
    public void Register<TSource, TDestination>(ICastMapper<TSource, TDestination> mapper)
    {
        ArgumentNullException.ThrowIfNull(mapper);

        var key = (typeof(TSource), typeof(TDestination));
        _mappers[key] = mapper;

        // Pre-compile the mapping function for performance
        _compiledMappers[key] = obj => mapper.Map((TSource)obj)!;
    }

    /// <summary>
    /// Registers a mapper using a delegate function.
    /// </summary>
    public void Register<TSource, TDestination>(Func<TSource, TDestination> mapFunc)
    {
        ArgumentNullException.ThrowIfNull(mapFunc);
        Register(new DelegateMapper<TSource, TDestination>(mapFunc));
    }

    /// <summary>
    /// Registers an async mapper using a delegate function.
    /// </summary>
    public void Register<TSource, TDestination>(
        Func<TSource, TDestination> mapFunc,
        Func<TSource, CancellationToken, Task<TDestination>> mapAsyncFunc)
    {
        ArgumentNullException.ThrowIfNull(mapFunc);
        ArgumentNullException.ThrowIfNull(mapAsyncFunc);
        Register(new AsyncDelegateMapper<TSource, TDestination>(mapFunc, mapAsyncFunc));
    }

    /// <summary>
    /// Retrieves a registered mapper for the specified type pair.
    /// </summary>
    public ICastMapper<TSource, TDestination> Get<TSource, TDestination>()
    {
        var key = (typeof(TSource), typeof(TDestination));

        if (_mappers.TryGetValue(key, out var mapper))
        {
            return (ICastMapper<TSource, TDestination>)mapper;
        }

        throw new MapperNotFoundException(typeof(TSource), typeof(TDestination));
    }

    /// <summary>
    /// Attempts to retrieve a registered mapper for the specified type pair.
    /// </summary>
    public bool TryGet<TSource, TDestination>(out ICastMapper<TSource, TDestination>? mapper)
    {
        var key = (typeof(TSource), typeof(TDestination));

        if (_mappers.TryGetValue(key, out var mapperObj))
        {
            mapper = (ICastMapper<TSource, TDestination>)mapperObj;
            return true;
        }

        mapper = null;
        return false;
    }

    /// <summary>
    /// Gets the compiled mapping function for performance-critical scenarios.
    /// </summary>
    internal Func<object, object> GetCompiledMapper(Type sourceType, Type destinationType)
    {
        var key = (sourceType, destinationType);

        if (_compiledMappers.TryGetValue(key, out var compiled))
        {
            return compiled;
        }

        throw new MapperNotFoundException(sourceType, destinationType);
    }

    /// <summary>
    /// Checks if a mapper is registered for the specified type pair.
    /// </summary>
    public bool IsRegistered<TSource, TDestination>()
    {
        var key = (typeof(TSource), typeof(TDestination));
        return _mappers.ContainsKey(key);
    }

    /// <summary>
    /// Removes all registered mappers.
    /// </summary>
    public void Clear()
    {
        _mappers.Clear();
        _compiledMappers.Clear();
    }
}