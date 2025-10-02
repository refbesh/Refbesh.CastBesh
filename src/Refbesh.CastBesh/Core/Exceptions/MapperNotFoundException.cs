namespace Refbesh.CastBesh.Core.Exceptions;

/// <summary>
/// Thrown when a mapper is not registered for the requested type pair.
/// </summary>
public class MapperNotFoundException : CastBeshException
{
    public Type SourceType { get; }
    public Type DestinationType { get; }

    public MapperNotFoundException(Type sourceType, Type destinationType)
        : base($"No mapper registered for casting from {sourceType.Name} to {destinationType.Name}")
    {
        SourceType = sourceType;
        DestinationType = destinationType;
    }
}