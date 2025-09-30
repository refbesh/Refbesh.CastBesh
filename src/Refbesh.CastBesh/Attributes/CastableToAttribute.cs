namespace Refbesh.CastBesh.Attributes;

/// <summary>
/// Marks a type as castable. Use this attribute to generate cast operators via source generators.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class CastableToAttribute : Attribute
{
    public Type DestinationType { get; }

    public CastableToAttribute(Type destinationType)
    {
        DestinationType = destinationType ?? throw new ArgumentNullException(nameof(destinationType));
    }
}