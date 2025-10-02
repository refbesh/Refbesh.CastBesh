namespace Refbesh.CastBesh.Attributes;

/// <summary>
/// Marks a type as able to receive casts from another type.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class CastableFromAttribute : Attribute
{
    public Type SourceType { get; }

    public CastableFromAttribute(Type sourceType)
    {
        SourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
    }
}