using Refbesh.CastBesh.Core;
using Refbesh.CastBesh.Registry;

namespace Refbesh.CastBesh.Configuration;

/// <summary>
/// Fluent configuration builder for registering cast mappers.
/// </summary>
public sealed class CastConfiguration
{
    private readonly CastMapperRegistry _registry;

    public CastConfiguration() : this(CastMapperRegistry.Instance) { }

    internal CastConfiguration(CastMapperRegistry registry)
    {
        _registry = registry;
    }

    /// <summary>
    /// Creates a new mapping configuration for the specified type pair.
    /// </summary>
    public MappingConfigurator<TSource, TDestination> CreateMap<TSource, TDestination>()
    {
        return new MappingConfigurator<TSource, TDestination>(this, _registry);
    }

    /// <summary>
    /// Registers a custom mapper instance.
    /// </summary>
    public CastConfiguration RegisterMapper<TSource, TDestination>(
        ICastMapper<TSource, TDestination> mapper)
    {
        _registry.Register(mapper);
        return this;
    }

    /// <summary>
    /// Clears all registered mappings.
    /// </summary>
    public CastConfiguration Clear()
    {
        _registry.Clear();
        return this;
    }
}
