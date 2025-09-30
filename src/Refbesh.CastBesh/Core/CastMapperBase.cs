// Core/CastMapperBase.cs
namespace Refbesh.CastBesh.Core;

/// <summary>
/// Base implementation for cast mappers with common functionality.
/// </summary>
public abstract class CastMapperBase<TSource, TDestination> : ICastMapper<TSource, TDestination>
{
    public abstract TDestination Map(TSource source);

    public virtual Task<TDestination> MapAsync(TSource source, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
            return Task.FromCanceled<TDestination>(cancellationToken);

        try
        {
            var result = Map(source);
            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            return Task.FromException<TDestination>(ex);
        }
    }
}