using Refbesh.CastBesh.Registry;

namespace Refbesh.CastBesh.Configuration;

/// <summary>
/// Fluent configurator for individual type mappings.
/// </summary>
public sealed class MappingConfigurator<TSource, TDestination>
{
    private readonly CastConfiguration _configuration;
    private readonly CastMapperRegistry _registry;
    private Func<TSource, TDestination>? _syncMapper;
    private Func<TSource, CancellationToken, Task<TDestination>>? _asyncMapper;

    internal MappingConfigurator(CastConfiguration configuration, CastMapperRegistry registry)
    {
        _configuration = configuration;
        _registry = registry;
    }

    /// <summary>
    /// Defines the synchronous mapping function.
    /// </summary>
    public MappingConfigurator<TSource, TDestination> WithSync(Func<TSource, TDestination> mapper)
    {
        _syncMapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        return this;
    }

    /// <summary>
    /// Defines the asynchronous mapping function.
    /// </summary>
    public MappingConfigurator<TSource, TDestination> WithAsync(
        Func<TSource, CancellationToken, Task<TDestination>> mapper)
    {
        _asyncMapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        return this;
    }

    /// <summary>
    /// Completes the mapping configuration and returns to the main configuration.
    /// </summary>
    public CastConfiguration Build()
    {
        if (_syncMapper == null)
        {
            throw new InvalidOperationException(
                $"No sync mapper defined for {typeof(TSource).Name} -> {typeof(TDestination).Name}");
        }

        if (_asyncMapper != null)
        {
            _registry.Register(_syncMapper, _asyncMapper);
        }
        else
        {
            _registry.Register(_syncMapper);
        }

        return _configuration;
    }
}