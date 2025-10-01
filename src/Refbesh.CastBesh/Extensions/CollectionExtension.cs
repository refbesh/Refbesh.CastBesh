using Refbesh.CastBesh.Registry;

namespace Refbesh.CastBesh.Extensions;

/// <summary>
/// Extension methods for collection mapping with optimized performance.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Maps a collection to List&lt;TDestination&gt; using registered mapper.
    /// </summary>
    public static List<TDestination> CastToList<TSource, TDestination>(
        this IEnumerable<TSource> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var mapper = CastMapperRegistry.Instance.GetCompiledMapper<TSource, TDestination>();

        // Optimize for known collection types
        if (source is ICollection<TSource> collection)
        {
            var result = new List<TDestination>(collection.Count);
            foreach (var item in collection)
            {
                result.Add(mapper(item));
            }
            return result;
        }

        return source.Select(mapper).ToList();
    }

    /// <summary>
    /// Maps a collection to TDestination[] using registered mapper.
    /// </summary>
    public static TDestination[] CastToArray<TSource, TDestination>(
        this IEnumerable<TSource> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var mapper = CastMapperRegistry.Instance.GetCompiledMapper<TSource, TDestination>();

        // Use ToArray for better performance
        if (source is ICollection<TSource> collection)
        {
            var result = new TDestination[collection.Count];
            var index = 0;
            foreach (var item in collection)
            {
                result[index++] = mapper(item);
            }
            return result;
        }

        return source.Select(mapper).ToArray();
    }

    /// <summary>
    /// Maps a collection asynchronously with parallel processing.
    /// </summary>
    public static async Task<List<TDestination>> CastToListAsync<TSource, TDestination>(
        this IEnumerable<TSource> source,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);

        var mapper = CastMapperRegistry.Instance.Get<TSource, TDestination>();
        var tasks = source.Select(s => mapper.MapAsync(s, cancellationToken));
        var results = await Task.WhenAll(tasks).ConfigureAwait(false);
        return results.ToList();
    }

    /// <summary>
    /// Maps a collection asynchronously in batches to control memory usage.
    /// </summary>
    public static async Task<List<TDestination>> CastToListAsync<TSource, TDestination>(
        this IEnumerable<TSource> source,
        int batchSize,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (batchSize <= 0) throw new ArgumentOutOfRangeException(nameof(batchSize));

        var mapper = CastMapperRegistry.Instance.Get<TSource, TDestination>();
        var result = new List<TDestination>();

        var batch = new List<TSource>(batchSize);
        foreach (var item in source)
        {
            batch.Add(item);

            if (batch.Count >= batchSize)
            {
                var tasks = batch.Select(s => mapper.MapAsync(s, cancellationToken));
                var batchResults = await Task.WhenAll(tasks).ConfigureAwait(false);
                result.AddRange(batchResults);
                batch.Clear();
            }
        }

        // Process remaining items
        if (batch.Count > 0)
        {
            var tasks = batch.Select(s => mapper.MapAsync(s, cancellationToken));
            var batchResults = await Task.WhenAll(tasks).ConfigureAwait(false);
            result.AddRange(batchResults);
        }

        return result;
    }
}