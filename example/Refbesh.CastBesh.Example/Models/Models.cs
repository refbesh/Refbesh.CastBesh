using Refbesh.CastBesh;
using Refbesh.CastBesh.Casting;

// ============================================
// Step 1: Define Entities and DTOs with Cast Operators
// ============================================
namespace Refbesh.CastBesh.Example.Models;
public class UserEntity
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }

    // Explicit cast operator to UserDto
    public static explicit operator UserDto(UserEntity entity)
    {
        return CastEngine.Cast<UserEntity, UserDto>(entity);
    }
}

public class UserDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;

    // Explicit cast operator from UserDto to UserEntity
    public static explicit operator UserEntity(UserDto dto)
    {
        return CastEngine.Cast<UserDto, UserEntity>(dto);
    }
}

public class ProductEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }

    // Explicit cast operator to ProductDto
    public static explicit operator ProductDto(ProductEntity entity)
    {
        return CastEngine.Cast<ProductEntity, ProductDto>(entity);
    }
}

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayPrice { get; set; } = string.Empty;
    public bool InStock { get; set; }

    // Explicit cast operator from ProductDto to ProductEntity
    public static explicit operator ProductEntity(ProductDto dto)
    {
        return CastEngine.Cast<ProductDto, ProductEntity>(dto);
    }
}