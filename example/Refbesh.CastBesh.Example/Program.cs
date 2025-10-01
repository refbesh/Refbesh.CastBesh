using Refbesh.CastBesh;
using Refbesh.CastBesh.Example.Models;
using Refbesh.CastBesh.Casting;
using Refbesh.CastBesh.Extensions;
using Refbesh.CastBesh.Example;
using Refbesh.CastBesh.Diagnostics;

Console.WriteLine("Initializing CastBesh...\n");
Startup.ConfigureMappings();

Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║    CastBesh - Native Cast Syntax via Source Generator         ║");
Console.WriteLine("║    Just like (int)myDouble but for complex types!             ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

// ============================================
// Example 1: EXPLICIT CAST SYNTAX (Main Feature!)
// ============================================
Console.WriteLine("═══ Example 1: Explicit Cast Syntax (Like Primitives!) ═══\n");

var userEntity = new UserEntity
{
    Id = 1,
    FirstName = "John",
    LastName = "Doe",
    Email = "john.doe@example.com",
    CreatedAt = DateTime.UtcNow.AddYears(-2),
    IsActive = true,
    PhoneNumber = "+1-555-0123",
    LastLoginDate = DateTime.UtcNow.AddHours(-3)
};

Console.WriteLine("Compare with primitive casting:");
double myDouble = 10.7;
int myInt = (int)myDouble;
Console.WriteLine($"  Primitive: (int){myDouble} = {myInt}");

// YOUR MAIN FEATURE - Cast objects like primitives!
var userDto = (UserDto)userEntity;  // ⭐ EXPLICIT CAST SYNTAX!
Console.WriteLine($"  CastBesh:  (UserDto)userEntity = {userDto.FullName}\n");

Console.WriteLine("Result:");
Console.WriteLine($"  Original: {userEntity.FirstName} {userEntity.LastName}");
Console.WriteLine($"  Casted:   {userDto.FullName} ({userDto.Status})\n");

// ============================================
// Example 2: Reverse Casting
// ============================================
Console.WriteLine("═══ Example 2: Bidirectional Casting ═══\n");

var newDto = new UserDto
{
    Id = 2,
    FullName = "Jane Smith",
    Email = "jane.smith@example.com",
    Status = "Active",
    Phone = "+1-555-9876"
};

Console.WriteLine($"Original DTO: {newDto.FullName}");

// Cast DTO back to Entity
var newEntity = (UserEntity)newDto;  // ⭐ REVERSE CAST!
Console.WriteLine($"Casted Entity: {newEntity.FirstName} {newEntity.LastName}");
Console.WriteLine($"IsActive: {newEntity.IsActive}\n");

// ============================================
// Example 3: Round-Trip Casting
// ============================================
Console.WriteLine("═══ Example 3: Round-Trip Casting ═══\n");

var original = new UserEntity
{
    Id = 99,
    FirstName = "Test",
    LastName = "User",
    Email = "test@example.com",
    IsActive = true
};

// Entity -> DTO -> Entity (multiple casts)
var intermediate = (UserDto)original;
var roundTrip = (UserEntity)intermediate;

Console.WriteLine($"Original:   {original.FirstName} {original.LastName}");
Console.WriteLine($"After DTO:  {intermediate.FullName}");
Console.WriteLine($"Round-trip: {roundTrip.FirstName} {roundTrip.LastName}\n");

// ============================================
// Example 4: Multiple Target Types
// ============================================
Console.WriteLine("═══ Example 4: One Entity, Multiple DTOs ═══\n");

// Cast same entity to different DTO types
var fullDto = (UserDto)userEntity;
var listDto = (UserListDto)userEntity;

Console.WriteLine($"Full DTO:  {fullDto.FullName} | {fullDto.Email}");
Console.WriteLine($"List DTO:  {listDto.Name} | {listDto.Status}\n");

// ============================================
// Example 5: Collections with Cast Syntax
// ============================================
Console.WriteLine("═══ Example 5: Collections ═══\n");

var users = new List<UserEntity>
{
    new() { Id = 1, FirstName = "Alice", LastName = "Johnson", Email = "alice@ex.com", IsActive = true },
    new() { Id = 2, FirstName = "Bob", LastName = "Williams", Email = "bob@ex.com", IsActive = false },
    new() { Id = 3, FirstName = "Carol", LastName = "Brown", Email = "carol@ex.com", IsActive = true },
    new() { Id = 4, FirstName = "David", LastName = "Miller", Email = "david@ex.com", IsActive = true }
};

// Cast each item using explicit cast syntax in LINQ
var dtos = users.Select(e => (UserDto)e).ToList();  // ⭐ CAST IN LINQ!
Console.WriteLine($"Casted {dtos.Count} users:");
foreach (var dto in dtos.Take(3))
{
    Console.WriteLine($"  • {dto.FullName}: {dto.Status}");
}
Console.WriteLine();

// Cast to different DTO type
var listDtos = users.Select(e => (UserListDto)e).ToList();
Console.WriteLine("As List DTOs:");
foreach (var dto in listDtos.Take(3))
{
    Console.WriteLine($"  • {dto.Name} {dto.Status}");
}
Console.WriteLine();

// ============================================
// Example 6: Products
// ============================================
Console.WriteLine("═══ Example 6: Product Casting ═══\n");

var product = new ProductEntity
{
    Id = 101,
    Name = "Laptop",
    Description = "High-performance laptop",
    Price = 1299.99m,
    Stock = 15,
    Category = "Electronics"
};

// Cast to different DTO types
var productDto = (ProductDto)product;  // ⭐ EXPLICIT CAST!
var detailedDto = (ProductDetailDto)product;  // ⭐ DIFFERENT TYPE!

Console.WriteLine($"Basic DTO:    {productDto.Name} - {productDto.DisplayPrice}");
Console.WriteLine($"Detailed DTO: {detailedDto.Name} - {detailedDto.Availability}\n");

// ============================================
// Example 7: Method Parameters
// ============================================
Console.WriteLine("═══ Example 7: Inline Casting in Methods ═══\n");

// Cast directly in method calls
ProcessUserDto((UserDto)userEntity);  // ⭐ INLINE CAST!
ProcessUserEntity((UserEntity)userDto);  // ⭐ INLINE REVERSE!
ProcessProductDto((ProductDto)product);  // ⭐ INLINE CAST!
Console.WriteLine();

// ============================================
// Example 8: Pattern Matching
// ============================================
Console.WriteLine("═══ Example 8: Pattern Matching with Casts ═══\n");

var entities = new List<object> { userEntity, product };

foreach (var entity in entities)
{
    if (entity is UserEntity user)
    {
        var dto = (UserDto)user;  // ⭐ CAST IN CONDITION!
        Console.WriteLine($"  User: {dto.FullName}");
    }
    else if (entity is ProductEntity prod)
    {
        var dto = (ProductDto)prod;  // ⭐ CAST IN CONDITION!
        Console.WriteLine($"  Product: {dto.Name} - {dto.DisplayPrice}");
    }
}
Console.WriteLine();

// ============================================
// Example 9: Complex Objects
// ============================================
Console.WriteLine("═══ Example 9: Complex Objects ═══\n");

var order = new OrderEntity
{
    Id = 1001,
    UserId = 1,
    OrderDate = DateTime.UtcNow,
    TotalAmount = 1419.97m,
    Status = "Shipped",
    Items = new List<OrderItemEntity>
    {
        new() { ProductId = 101, ProductName = "Laptop", Quantity = 1, UnitPrice = 1299.99m },
        new() { ProductId = 102, ProductName = "Mouse", Quantity = 4, UnitPrice = 29.99m }
    }
};

var orderDto = (OrderDto)order;  // ⭐ CAST COMPLEX OBJECT!
Console.WriteLine($"Order #{orderDto.Id}");
Console.WriteLine($"  Total: {orderDto.TotalAmount}");
Console.WriteLine($"  Items: {orderDto.ItemCount}");
Console.WriteLine($"  Status: {orderDto.Status}\n");

// ============================================
// Example 10: Chained Operations
// ============================================
Console.WriteLine("═══ Example 10: Chained Operations ═══\n");

var result = users
    .Where(u => u.IsActive)
    .Select(u => (UserDto)u)  // ⭐ CAST IN PIPELINE!
    .OrderBy(d => d.FullName)
    .Take(3)
    .ToList();

Console.WriteLine("Active users (sorted):");
foreach (var dto in result)
{
    Console.WriteLine($"  • {dto.FullName}");
}
Console.WriteLine();

// ============================================
// Example 11: Alternative Syntax Options
// ============================================
Console.WriteLine("═══ Example 11: Alternative Syntax ═══\n");

Console.WriteLine("All these work:");

// Option 1: Explicit cast (main feature)
var dto1 = (UserDto)userEntity;
Console.WriteLine($"  1. (UserDto)entity: {dto1.FullName}");

// Option 2: .As<T>() extension
var dto2 = userEntity.As<UserDto>();
Console.WriteLine($"  2. .As<UserDto>():  {dto2.FullName}");

// Option 3: .ToCastable().To<T>()
var dto3 = userEntity.ToCastable().To<UserDto>();
Console.WriteLine($"  3. .ToCastable():   {dto3.FullName}");

Console.WriteLine("\nRecommended: Use (TargetType)source for best readability!\n");

// ============================================
// Example 12: Performance
// ============================================
Console.WriteLine("═══ Example 12: Performance ═══\n");

MappingDiagnostics.Enabled = true;

// Perform 10,000 casts
for (int i = 0; i < 10000; i++)
{
    var _ = (UserDto)userEntity;  // ⭐ EXPLICIT CAST!
}

var metrics = MappingDiagnostics.GetMetrics<UserEntity, UserDto>();
if (metrics != null)
{
    Console.WriteLine($"Performed {metrics.TotalMappings:N0} casts");
    Console.WriteLine($"Average: {metrics.AverageDuration.TotalMilliseconds:F4} ms");
    Console.WriteLine($"Fastest: {metrics.FastestMapping.TotalMilliseconds:F4} ms");
    Console.WriteLine($"Success: {metrics.SuccessRate:F1}%");
}

Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║                   Examples Complete!                           ║");
Console.WriteLine("║                                                                ║");
Console.WriteLine("║  Main Feature: (TargetType)source - like primitive casts!     ║");
Console.WriteLine("║  No manual operators - Source Generator does it all!          ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

// Helper methods
static void ProcessUserDto(UserDto dto)
{
    Console.WriteLine($"  → Processing DTO: {dto.FullName}");
}

static void ProcessUserEntity(UserEntity entity)
{
    Console.WriteLine($"  → Processing Entity: {entity.FirstName} {entity.LastName}");
}

static void ProcessProductDto(ProductDto dto)
{
    Console.WriteLine($"  → Processing Product: {dto.Name} ({dto.DisplayPrice})");
}