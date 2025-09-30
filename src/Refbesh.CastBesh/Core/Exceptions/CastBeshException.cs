namespace Refbesh.CastBesh.Core.Exceptions;

/// <summary>
/// Base exception for all CastBesh-related errors.
/// </summary>
public class CastBeshException : Exception
{
    public CastBeshException(string message) : base(message) { }
    public CastBeshException(string message, Exception innerException) : base(message, innerException) { }
}