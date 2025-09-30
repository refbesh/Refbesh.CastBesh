using Refbesh.CastBesh.Core;

namespace Refbesh.CastBesh.Registry;

/// <summary>
/// Mapper implementation with both sync and async delegates.
/// </summary>
internal sealed class AsyncDelegateMapper<TSource, TDestination> : CastMapperBase<TSource, TDestination>
{
    private readonly Func<TSource, TDestination> _mapFunc;
    private readonly Func<TSource, CancellationToken, Task<TDestination>> _mapAsyncFunc;

    public AsyncDelegateMapper(
        Func<TSource, TDestination> mapFunc,
        Func<TSource, CancellationToken, Task<TDestination>> mapAsyncFunc)
    {
        _mapFunc = mapFunc ?? throw new ArgumentNullException(nameof(mapFunc));
        _mapAsyncFunc = mapAsyncFunc ?? throw new ArgumentNullException(nameof(mapAsyncFunc));
    }

    public override TDestination Map(TSource source)
    {
        try
        {
            return _mapFunc(source);
        }
        catch (Exception ex)
        {
            throw new Core.Exceptions.MappingException(
                typeof(TSource),
                typeof(TDestination),
                ex.Message,
                ex);
        }
    }

    public override async Task<TDestination> MapAsync(TSource source, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _mapAsyncFunc(source, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            throw new Core.Exceptions.MappingException(
                typeof(TSource),
                typeof(TDestination),
                ex.Message,
                ex);
        }
    }
}
