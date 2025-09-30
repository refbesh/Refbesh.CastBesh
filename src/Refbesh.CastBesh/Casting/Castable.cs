using Refbesh.CastBesh.Registry;

namespace Refbesh.CastBesh.Casting;

/// <summary>
/// Wrapper type that enables explicit cast operator syntax.
/// Usage: var dto = (UserDto)Castable.From(entity);
/// </summary>
public readonly struct Castable
{
    private readonly object _value;

    private Castable(object value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Creates a Castable wrapper from any object.
    /// </summary>
    public static Castable From(object value) => new(value);

    /// <summary>
    /// Gets the underlying value.
    /// </summary>
    public object Value => _value;

    /// <summary>
    /// Explicit cast to destination type - this is the magic!
    /// Users can write: var dto = (UserDto)Castable.From(entity);
    /// </summary>
    public T CastTo<T>()
    {
        var sourceType = _value.GetType();
        var mapper = CastMapperRegistry.Instance.GetCompiledMapper(sourceType, typeof(T));
        return (T)mapper(_value);
    }
}