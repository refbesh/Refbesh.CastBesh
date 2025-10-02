using System.Reflection;

namespace Refbesh.CastBesh.Configuration;

/// <summary>
/// Validates mapping configurations at startup.
/// </summary>
public static class ConfigurationValidator
{
    /// <summary>
    /// Validates that a mapping between TSource and TDestination is feasible.
    /// </summary>
    public static ValidationResult Validate<TSource, TDestination>()
    {
        return Validate(typeof(TSource), typeof(TDestination));
    }

    /// <summary>
    /// Validates that a mapping between source and destination types is feasible.
    /// </summary>
    public static ValidationResult Validate(Type sourceType, Type destinationType)
    {
        var result = new ValidationResult();

        // Check if destination type has parameterless constructor
        var destConstructor = destinationType.GetConstructor(Type.EmptyTypes);
        if (destConstructor == null && destinationType.IsClass && !destinationType.IsAbstract)
        {
            result.AddError($"{destinationType.Name} must have a parameterless constructor");
        }

        var sourceProps = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead)
            .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

        var destProps = destinationType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite)
            .ToList();

        if (destProps.Count == 0)
        {
            result.AddWarning($"{destinationType.Name} has no writable properties");
        }

        int mappedCount = 0;
        foreach (var destProp in destProps)
        {
            if (!sourceProps.TryGetValue(destProp.Name, out var sourceProp))
            {
                result.AddWarning($"No matching source property for {destProp.Name}");
                continue;
            }

            if (!AreTypesCompatible(sourceProp.PropertyType, destProp.PropertyType))
            {
                result.AddWarning(
                    $"Type mismatch for {destProp.Name}: {sourceProp.PropertyType.Name} -> {destProp.PropertyType.Name}");
            }
            else
            {
                mappedCount++;
            }
        }

        if (mappedCount == 0)
        {
            result.AddError($"No compatible properties found between {sourceType.Name} and {destinationType.Name}");
        }

        return result;
    }

    private static bool AreTypesCompatible(Type source, Type destination)
    {
        if (destination.IsAssignableFrom(source))
            return true;

        var underlyingSource = Nullable.GetUnderlyingType(source) ?? source;
        var underlyingDest = Nullable.GetUnderlyingType(destination) ?? destination;

        if (underlyingDest.IsAssignableFrom(underlyingSource))
            return true;

        if (underlyingSource.IsPrimitive && underlyingDest.IsPrimitive)
            return true;

        if (underlyingDest == typeof(string))
            return true;

        return false;
    }
}

/// <summary>
/// Result of configuration validation.
/// </summary>
public sealed class ValidationResult
{
    private readonly List<string> _warnings = new();
    private readonly List<string> _errors = new();

    public IReadOnlyList<string> Warnings => _warnings;
    public IReadOnlyList<string> Errors => _errors;
    public bool IsValid => _errors.Count == 0;
    public bool HasWarnings => _warnings.Count > 0;

    internal void AddWarning(string message) => _warnings.Add(message);
    internal void AddError(string message) => _errors.Add(message);

    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();

        if (_errors.Count > 0)
        {
            sb.AppendLine("Errors:");
            foreach (var error in _errors)
                sb.AppendLine($"  - {error}");
        }

        if (_warnings.Count > 0)
        {
            sb.AppendLine("Warnings:");
            foreach (var warning in _warnings)
                sb.AppendLine($"  - {warning}");
        }

        return sb.ToString();
    }
}