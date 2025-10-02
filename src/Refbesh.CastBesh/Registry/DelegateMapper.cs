using Refbesh.CastBesh.Core;

namespace Refbesh.CastBesh.Registry;

/// <summary>
/// Mapper implementation using delegates for flexibility.
/// </summary>
internal sealed class DelegateMapper<TSource, TDestination> : CastMapperBase<TSource, TDestination>
{
    private readonly Func<TSource, TDestination> _mapFunc;

    public DelegateMapper(Func<TSource, TDestination> mapFunc)
    {
        _mapFunc = mapFunc ?? throw new ArgumentNullException(nameof(mapFunc));
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
}