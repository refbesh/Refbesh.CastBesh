using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Refbesh.CastBesh.Core;
using Refbesh.CastBesh.Core.Exceptions;

namespace Refbesh.CastBesh.Registry;

public sealed class CastMapperRegistry
{
    private static readonly Lazy<CastMapperRegistry> _instance =
        new(() => new CastMapperRegistry(), LazyThreadSafetyMode.ExecutionAndPublication);

    // CHANGED: Use struct key for better performance (no allocations)
    private readonly ConcurrentDictionary<MapperKey, object> _mappers;
    private readonly ConcurrentDictionary<MapperKey, Delegate> _compiledMappers;

    public static CastMapperRegistry Instance => _instance.Value;

    private CastMapperRegistry()
    {
        _mappers = new ConcurrentDictionary<MapperKey, object>();
        _compiledMappers = new ConcurrentDictionary<MapperKey, Delegate>();
    }

    // CHANGED: Added AggressiveInlining for performance
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Register<TSource, TDestination>(ICastMapper<TSource, TDestination> mapper)
    {
        ArgumentNullException.ThrowIfNull(mapper);

        var key = new MapperKey(typeof(TSource), typeof(TDestination));
        _mappers[key] = mapper;

        // CHANGED: Store strongly-typed delegate to avoid boxing
        _compiledMappers[key] = new Func<TSource, TDestination>(mapper.Map);
    }

    // NEW: Auto-registration using expression mapper
    public void RegisterAutoMap<TSource, TDestination>() where TDestination : new()
    {
        Register(new ExpressionMapper<TSource, TDestination>());
    }

    public void Register<TSource, TDestination>(Func<TSource, TDestination> mapFunc)
    {
        ArgumentNullException.ThrowIfNull(mapFunc);
        Register(new DelegateMapper<TSource, TDestination>(mapFunc));
    }

    public void Register<TSource, TDestination>(
        Func<TSource, TDestination> mapFunc,
        Func<TSource, CancellationToken, Task<TDestination>> mapAsyncFunc)
    {
        ArgumentNullException.ThrowIfNull(mapFunc);
        ArgumentNullException.ThrowIfNull(mapAsyncFunc);
        Register(new AsyncDelegateMapper<TSource, TDestination>(mapFunc, mapAsyncFunc));
    }

    // CHANGED: Added AggressiveInlining and Unsafe.As for performance
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ICastMapper<TSource, TDestination> Get<TSource, TDestination>()
    {
        var key = new MapperKey(typeof(TSource), typeof(TDestination));

        if (_mappers.TryGetValue(key, out var mapper))
        {
            return Unsafe.As<ICastMapper<TSource, TDestination>>(mapper);
        }

        throw new MapperNotFoundException(typeof(TSource), typeof(TDestination));
    }

    public bool TryGet<TSource, TDestination>(out ICastMapper<TSource, TDestination>? mapper)
    {
        var key = new MapperKey(typeof(TSource), typeof(TDestination));

        if (_mappers.TryGetValue(key, out var mapperObj))
        {
            mapper = (ICastMapper<TSource, TDestination>)mapperObj;
            return true;
        }

        mapper = null;
        return false;
    }

    // CHANGED: Now returns strongly-typed Func instead of Func<object, object>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Func<TSource, TDestination> GetCompiledMapper<TSource, TDestination>()
    {
        var key = new MapperKey(typeof(TSource), typeof(TDestination));

        if (_compiledMappers.TryGetValue(key, out var compiled))
        {
            return Unsafe.As<Func<TSource, TDestination>>(compiled);
        }

        throw new MapperNotFoundException(typeof(TSource), typeof(TDestination));
    }

    // KEPT: Old version for backward compatibility
    internal Func<object, object> GetCompiledMapper(Type sourceType, Type destinationType)
    {
        var key = new MapperKey(sourceType, destinationType);

        if (_compiledMappers.TryGetValue(key, out var compiled))
        {
            return obj => compiled.DynamicInvoke(obj)!;
        }

        throw new MapperNotFoundException(sourceType, destinationType);
    }

    public bool IsRegistered<TSource, TDestination>()
    {
        var key = new MapperKey(typeof(TSource), typeof(TDestination));
        return _mappers.ContainsKey(key);
    }

    public void Clear()
    {
        _mappers.Clear();
        _compiledMappers.Clear();
    }

    // NEW: Struct key to avoid heap allocations
    private readonly struct MapperKey : IEquatable<MapperKey>
    {
        private readonly Type _source;
        private readonly Type _destination;
        private readonly int _hashCode;

        public MapperKey(Type source, Type destination)
        {
            _source = source;
            _destination = destination;
            _hashCode = HashCode.Combine(source, destination);
        }

        public bool Equals(MapperKey other) =>
            _source == other._source && _destination == other._destination;

        public override bool Equals(object? obj) =>
            obj is MapperKey other && Equals(other);

        public override int GetHashCode() => _hashCode;
    }
}