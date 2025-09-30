namespace Refbesh.CastBesh.Core.Exceptions;

/// <summary>
/// Thrown when an error occurs during the mapping process.
/// </summary>
public class MappingException : CastBeshException
{
    public Type SourceType { get; }
    public Type DestinationType { get; }

    public MappingException(Type sourceType, Type destinationType, string message, Exception? innerException = null)
        : base($"Error mapping from {sourceType.Name} to {destinationType.Name}: {message}", innerException!)
    {
        SourceType = sourceType;
        DestinationType = destinationType;
    }
}