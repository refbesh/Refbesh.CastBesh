using Refbesh.CastBesh;
using Refbesh.CastBesh.Example.Models;
using Refbesh.CastBesh.Casting;
using Refbesh.CastBesh.Example;
using Refbesh.CastBesh.Example.Models;

Startup.ConfigureMappings();

Console.WriteLine("=== Refbesh.CastBesh - Native Cast Syntax ===\n");

// ----------------------------------------
// Example 1: Entity to DTO (Like primitive cast!)
// ----------------------------------------
Console.WriteLine("1. Entity to DTO Cast:");

var userEntity = new UserEntity
{
    Id = 1,
    FirstName = "John",
    LastName = "Doe",
    Email = "john.doe@example.com",
    CreatedAt = DateTime.UtcNow,
    IsActive = true
};

// Native cast syntax - just like (int)myDouble!
var userDto = (UserDto)userEntity;

Console.WriteLine($"   Original: {userEntity.FirstName} {userEntity.LastName}");
Console.WriteLine($"   Casted DTO: {userDto.FullName} - {userDto.Email} [{userDto.Status}]\n");

// ----------------------------------------
// Example 2: DTO to Entity (Reverse cast!)
// ----------------------------------------
Console.WriteLine("2. DTO to Entity Cast:");

var newUserDto = new UserDto
{
    Id = 2,
    FullName = "Jane Smith",
    Email = "jane.smith@example.com",
    Status = "Active"
};

// Reverse cast - just like casting back!
var newUserEntity = (UserEntity)newUserDto;

Console.WriteLine($"   Original DTO: {newUserDto.FullName}");
Console.WriteLine($"   Casted Entity: {newUserEntity.FirstName} {newUserEntity.LastName}");
Console.WriteLine($"   IsActive: {newUserEntity.IsActive}\n");

// ----------------------------------------
// Example 3: Product Casting
// ----------------------------------------
Console.WriteLine("3. Product Entity to DTO:");

var productEntity = new ProductEntity
{
    Id = 100,
    Name = "Laptop",
    Price = 999.99m,
    Stock = 5
};

//  Cast product entity to DTO
var productDto = (ProductDto)productEntity;

Console.WriteLine($"   Product: {productDto.Name}");
Console.WriteLine($"   Price: {productDto.DisplayPrice}");
Console.WriteLine($"   In Stock: {productDto.InStock}\n");

// ----------------------------------------
// Example 4: Chained Operations with Casts
// ----------------------------------------
Console.WriteLine("4. Using Casts in Methods:");

ProcessUserDto((UserDto)userEntity);
ProcessUserEntity((UserEntity)userDto);
Console.WriteLine();

// ----------------------------------------
// Example 5: Collections with Cast Syntax
// ----------------------------------------
Console.WriteLine("5. Casting Collections:");

var userEntities = new List<UserEntity>
        {
            new() { Id = 1, FirstName = "Alice", LastName = "Johnson", Email = "alice@ex.com", IsActive = true },
            new() { Id = 2, FirstName = "Bob", LastName = "Williams", Email = "bob@ex.com", IsActive = false },
            new() { Id = 3, FirstName = "Carol", LastName = "Brown", Email = "carol@ex.com", IsActive = true }
        };

// Cast each entity using native syntax
var userDtos = userEntities.Select(e => (UserDto)e).ToList();

foreach (var dto in userDtos)
{
    Console.WriteLine($"   - {dto.FullName}: {dto.Status}");
}
Console.WriteLine();

// ----------------------------------------
// Example 6: Inline Casting in Expressions
// ----------------------------------------
Console.WriteLine("6. Inline Casting:");

// Cast directly in method calls
var result = CalculateUserScore((UserDto)userEntity);
Console.WriteLine($"   User Score: {result}\n");

// ----------------------------------------
// Example 7: Conditional Casting
// ----------------------------------------
Console.WriteLine("7. Conditional Casting:");

var entities = new List<object> { userEntity, productEntity };

foreach (var entity in entities)
{
    if (entity is UserEntity user)
    {
        var dto = (UserDto)user;
        Console.WriteLine($"   User: {dto.FullName}");
    }
    else if (entity is ProductEntity product)
    {
        var pdto = (ProductDto)product;
        Console.WriteLine($"   Product: {pdto.Name} - {pdto.DisplayPrice}");
    }
}
Console.WriteLine();

// ----------------------------------------
// Example 8: Async Operations (using CastEngine directly)
// ----------------------------------------
Console.WriteLine("8. Async Casting:");

var asyncDto = await CastEngine.CastAsync<UserEntity, UserDto>(userEntity);
Console.WriteLine($"   Async Result: {asyncDto.FullName}\n");

// ----------------------------------------
// Example 9: Round-trip Casting
// ----------------------------------------
Console.WriteLine("9. Round-trip Cast:");

var originalEntity = new UserEntity
{
    Id = 99,
    FirstName = "Test",
    LastName = "User",
    Email = "testtest@example.com",
    IsActive = true
};

// Entity -> DTO -> Entity
var intermediateDto = (UserDto)originalEntity;
var roundTripEntity = (UserEntity)intermediateDto;

Console.WriteLine($"   Original: {originalEntity.FirstName} {originalEntity.LastName}");
Console.WriteLine($"   After round-trip: {roundTripEntity.FirstName} {roundTripEntity.LastName}\n");

// ----------------------------------------
// Example 10: Mimicking Primitive Casts
// ----------------------------------------
Console.WriteLine("10. Compare with Primitive Casts:");

double myDouble = 10.7;
int myInt = (int)myDouble;  // Primitive cast
Console.WriteLine($"   Primitive: (int){myDouble} = {myInt}");

var entity1 = new UserEntity { Id = 1, FirstName = "Tom", LastName = "Ford", Email = "tom@ex.com", IsActive = true };
var dto1 = (UserDto)entity1;  // CastBesh cast - same syntax!
Console.WriteLine($"   CastBesh: (UserDto)entity = {dto1.FullName}");

Console.WriteLine("\n=== Examples Complete ===");

static void ProcessUserDto(UserDto dto)
{
    Console.WriteLine($"   Processing DTO: {dto.FullName}");
}

static void ProcessUserEntity(UserEntity entity)
{
    Console.WriteLine($"   Processing Entity: {entity.FirstName} {entity.LastName}");
}

static int CalculateUserScore(UserDto dto)
{
    return dto.Id * 10;
}