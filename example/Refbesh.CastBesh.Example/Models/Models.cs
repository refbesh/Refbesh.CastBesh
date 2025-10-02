using Refbesh.CastBesh.Casting;

namespace Refbesh.CastBesh.Example.Models;

// ============================================
// Models with Manual Cast Operators
// Simple and explicit - just one line per cast direction!
// ============================================

public class UserEntity
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? LastLoginDate { get; set; }

    // Cast operators - just one line!
    public static explicit operator UserDto(UserEntity source)
        => CastEngine.Cast<UserEntity, UserDto>(source);

    public static explicit operator UserListDto(UserEntity source)
        => CastEngine.Cast<UserEntity, UserListDto>(source);
}

public class UserDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? LastLogin { get; set; }

    // Reverse cast
    public static explicit operator UserEntity(UserDto source)
        => CastEngine.Cast<UserDto, UserEntity>(source);
}

public class UserListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class ProductEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Cast operators
    public static explicit operator ProductDto(ProductEntity source)
        => CastEngine.Cast<ProductEntity, ProductDto>(source);

    public static explicit operator ProductDetailDto(ProductEntity source)
        => CastEngine.Cast<ProductEntity, ProductDetailDto>(source);
}

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayPrice { get; set; } = string.Empty;
    public bool InStock { get; set; }
    public string Category { get; set; } = string.Empty;

    // Reverse cast
    public static explicit operator ProductEntity(ProductDto source)
        => CastEngine.Cast<ProductDto, ProductEntity>(source);
}

public class ProductDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string DisplayPrice { get; set; } = string.Empty;
    public int AvailableQuantity { get; set; }
    public string Availability { get; set; } = string.Empty;
}

public class OrderEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<OrderItemEntity> Items { get; set; } = new();

    // Cast operator
    public static explicit operator OrderDto(OrderEntity source)
        => CastEngine.Cast<OrderEntity, OrderDto>(source);
}

public class OrderItemEntity
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class OrderDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string OrderDate { get; set; } = string.Empty;
    public string TotalAmount { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int ItemCount { get; set; }
}