using System.Linq.Expressions;
using System.Reflection;
using Refbesh.CastBesh.Core.Exceptions;

namespace Refbesh.CastBesh.Core;

/// <summary>
/// High-performance mapper using compiled expression trees.
/// 10-50x faster than reflection-based mapping.
/// </summary>
public sealed class ExpressionMapper<TSource, TDestination> : CastMapperBase<TSource, TDestination>
    where TDestination : new()
{
    private readonly Func<TSource, TDestination> _compiledMapper;

    public ExpressionMapper()
    {
        _compiledMapper = BuildMapper();
    }

    public override TDestination Map(TSource source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        try
        {
            return _compiledMapper(source);
        }
        catch (Exception ex)
        {
            throw new MappingException(typeof(TSource), typeof(TDestination), ex.Message, ex);
        }
    }

    private static Func<TSource, TDestination> BuildMapper()
    {
        var sourceParam = Expression.Parameter(typeof(TSource), "source");
        var bindings = new List<MemberBinding>();

        var destinationProperties = typeof(TDestination)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite)
            .ToList();

        var sourceProperties = typeof(TSource)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead)
            .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

        foreach (var destProp in destinationProperties)
        {
            if (sourceProperties.TryGetValue(destProp.Name, out var sourceProp))
            {
                try
                {
                    Expression propertyAccess = Expression.Property(sourceParam, sourceProp);

                    // Handle type conversion
                    if (destProp.PropertyType != sourceProp.PropertyType)
                    {
                        if (destProp.PropertyType.IsAssignableFrom(sourceProp.PropertyType))
                        {
                            // Direct assignment (e.g., object from string)
                            bindings.Add(Expression.Bind(destProp, propertyAccess));
                        }
                        else if (IsConvertible(sourceProp.PropertyType, destProp.PropertyType))
                        {
                            // Value type conversion
                            var conversion = Expression.Convert(propertyAccess, destProp.PropertyType);
                            bindings.Add(Expression.Bind(destProp, conversion));
                        }
                        else if (TryGetImplicitConversion(sourceProp.PropertyType, destProp.PropertyType, out var conversionMethod))
                        {
                            // Implicit operator conversion
                            var conversion = Expression.Call(conversionMethod, propertyAccess);
                            bindings.Add(Expression.Bind(destProp, conversion));
                        }
                    }
                    else
                    {
                        // Same type, direct assignment
                        bindings.Add(Expression.Bind(destProp, propertyAccess));
                    }
                }
                catch
                {
                    // Skip properties that can't be mapped
                    continue;
                }
            }
        }

        if (bindings.Count == 0)
        {
            throw new InvalidOperationException(
                $"No mappable properties found between {typeof(TSource).Name} and {typeof(TDestination).Name}");
        }

        var memberInit = Expression.MemberInit(
            Expression.New(typeof(TDestination)),
            bindings
        );

        var lambda = Expression.Lambda<Func<TSource, TDestination>>(memberInit, sourceParam);
        return lambda.Compile();
    }

    private static bool IsConvertible(Type source, Type destination)
    {
        if (destination.IsAssignableFrom(source))
            return true;

        // Handle nullable types
        var underlyingSource = Nullable.GetUnderlyingType(source) ?? source;
        var underlyingDest = Nullable.GetUnderlyingType(destination) ?? destination;

        // Check for primitive type conversions
        if (underlyingSource.IsPrimitive && underlyingDest.IsPrimitive)
            return true;

        // Check for string conversions
        if (underlyingDest == typeof(string))
            return true;

        return false;
    }

    private static bool TryGetImplicitConversion(Type source, Type destination, out MethodInfo? method)
    {
        method = destination
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m =>
                m.Name == "op_Implicit" &&
                m.ReturnType == destination &&
                m.GetParameters().Length == 1 &&
                m.GetParameters()[0].ParameterType == source);

        return method != null;
    }
}