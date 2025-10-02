using Refbesh.CastBesh.Registry;
using System.Runtime.CompilerServices;

namespace Refbesh.CastBesh.Casting;

/// <summary>
/// Generic wrapper type for better performance with compile-time type safety.
/// Usage: var dto = Castable.From(entity).To&lt;UserDto&gt;();
/// </summary>
public readonly struct Castable<TSource>
{
    private readonly TSource _value;

    private Castable(TSource value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public static Castable<TSource> From(TSource value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TDestination To<TDestination>()
    {
        var mapper = CastMapperRegistry.Instance.GetCompiledMapper<TSource, TDestination>();
        return mapper(_value);
    }

    public TSource Value => _value;

    // Allow explicit cast from source type
    public static explicit operator Castable<TSource>(TSource value) => From(value);
}