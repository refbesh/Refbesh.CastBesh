# CastBesh Quick Start Guide

Get up and running with CastBesh in 5 minutes!

---

## 📦 Step 1: Install (30 seconds)

```bash
dotnet add package Refbesh.CastBesh
```

Or via Package Manager Console:
```powershell
Install-Package Refbesh.CastBesh
```

---

## 🏗️ Step 2: Add Cast Operators to Your Models (1 minute)

Add one simple line to each class for each cast direction:

```csharp
using Refbesh.CastBesh.Casting;

// Domain/Entities/User.cs
public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    // Add this one line!
    public static explicit operator UserDto(User source)
        => CastEngine.Cast<User, UserDto>(source);
}

// DTOs/UserDto.cs
public class UserDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    // Add reverse cast (optional)
    public static explicit operator User(UserDto source)
        => CastEngine.Cast<UserDto, User>(source);
}
```

✅ **That's it for the models!** Just one line per cast direction!

---

## ⚙️ Step 3: Configure Mappings at Startup (2 minutes)

In your `Program.cs` or `Startup.cs`:

```csharp
using Refbesh.CastBesh;

// At application startup (before any casting)
CastBeshStartup.Configure(config =>
{
    // Define how to map User -> UserDto
    config.CreateMap<User, UserDto>()
        .WithSync(user => new UserDto
        {
            Id = user.Id,
            FullName = $"{user.FirstName} {user.LastName}",
            Email = user.Email
        })
        .Build();
    
    // Optional: Add reverse mapping (UserDto -> User)
    config.CreateMap<UserDto, User>()
        .WithSync(dto =>
        {
            var names = dto.FullName.Split(' ', 2);
            return new User
            {
                Id = dto.Id,
                FirstName = names.Length > 0 ? names[0] : "",
                LastName = names.Length > 1 ? names[1] : "",
                Email = dto.Email
            };
        })
        .Build();
});
```

---

## 🚀 Step 4: Use It Everywhere! (1.5 minutes)

### Single Object

```csharp
var user = new User 
{ 
    Id = 1, 
    FirstName = "John", 
    LastName = "Doe",
    Email = "john@example.com" 
};

// Cast to DTO - just like primitive types!
var dto = (UserDto)user;

Console.WriteLine(dto.FullName); // "John Doe"
```

### Collections

```csharp
List<User> users = GetUsersFromDatabase();

// Cast entire collection using LINQ
var dtos = users.Select(u => (UserDto)u).ToList();

// Or use optimized extension method
using Refbesh.CastBesh.Extensions;
var dtos = users.CastToList<User, UserDto>();
```

### In Method Parameters

```csharp
void SendEmail(UserDto dto) 
{
    // Send email logic
}

// Cast inline - just like primitives!
SendEmail((UserDto)user);
```

### Reverse Casting

```csharp
// Cast DTO back to entity
var user = (User)dto;
```

### Async Operations

```csharp
var dto = await CastEngine.CastAsync<User, UserDto>(user);
```

---

## 🎉 You're Done!

Compare with primitive casting:

```csharp
// Primitive casting
double myDouble = 10.7;
int myInt = (int)myDouble;

// CastBesh - same syntax!
User user = GetUser();
UserDto dto = (UserDto)user;
```

---

## 🔥 Common Patterns

### ASP.NET Core Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repo;

    [HttpGet("{id}")]
    public ActionResult<UserDto> GetUser(int id)
    {
        var user = _repo.GetById(id);
        if (user == null) return NotFound();
        
        // Cast entity to DTO before returning
        return Ok((UserDto)user);
    }

    [HttpGet]
    public ActionResult<List<UserDto>> GetAllUsers()
    {
        var users = _repo.GetAll();
        
        // Cast collection
        return Ok(users.Select(u => (UserDto)u).ToList());
    }

    [HttpPost]
    public ActionResult<UserDto> CreateUser(UserDto dto)
    {
        // Cast DTO to entity
        var user = (User)dto;
        
        _repo.Add(user);
        
        return CreatedAtAction(
            nameof(GetUser), 
            new { id = user.Id }, 
            (UserDto)user  // Cast back to DTO
        );
    }
}
```

### Service Layer

```csharp
public class UserService
{
    private readonly IUserRepository _repo;

    public UserDto GetUserById(int id)
    {
        var user = _repo.GetById(id);
        return (UserDto)user;
    }

    public List<UserDto> GetActiveUsers()
    {
        return _repo.GetAll()
            .Where(u => u.IsActive)
            .Select(u => (UserDto)u)
            .ToList();
    }

    public void CreateUser(UserDto dto)
    {
        var user = (User)dto;
        _repo.Add(user);
    }
}
```

### LINQ Queries

```csharp
var results = dbContext.Users
    .Where(u => u.IsActive)
    .ToList()  // Execute query first
    .Select(u => (UserDto)u)  // Then cast in memory
    .OrderBy(d => d.FullName)
    .ToList();
```

---

## 💡 Pro Tips

### Tip 1: One Line Per Direction

```csharp
// Keep it simple - just one line!
public static explicit operator UserDto(User source)
    => CastEngine.Cast<User, UserDto>(source);
```

### Tip 2: Multiple Target Types

```csharp
public class User
{
    // Properties...
    
    // Cast to multiple DTOs
    public static explicit operator UserDto(User source)
        => CastEngine.Cast<User, UserDto>(source);
    
    public static explicit operator UserListDto(User source)
        => CastEngine.Cast<User, UserListDto>(source);
}

// Usage
var fullDto = (UserDto)user;
var listDto = (UserListDto)user;
```

### Tip 3: Organize Mappings by Feature

```csharp
// MappingProfiles/UserMappingProfile.cs
public static class UserMappingProfile
{
    public static void Configure(CastConfiguration config)
    {
        config.CreateMap<User, UserDto>()
            .WithSync(/* ... */)
            .Build();
            
        config.CreateMap<User, UserListDto>()
            .WithSync(/* ... */)
            .Build();
    }
}

// Startup
CastBeshStartup.Configure(config =>
{
    UserMappingProfile.Configure(config);
    ProductMappingProfile.Configure(config);
    OrderMappingProfile.Configure(config);
});
```

### Tip 4: When You Can't Modify a Type

If you can't add cast operators (e.g., third-party library):

```csharp
using Refbesh.CastBesh.Extensions;

// Use .As<T>() extension method instead
var dto = user.As<UserDto>();

// Or .ToCastable().To<T>()
var dto = user.ToCastable().To<UserDto>();
```

---

## ❓ Troubleshooting

### "Mapper not found" error?

**Solution:** Make sure you configured the mapping at startup:
```csharp
config.CreateMap<SourceType, DestinationType>()
    .WithSync(/* mapping */)
    .Build();
```

### "Cannot convert type" error?

**Solution:** Make sure you added the cast operator:
```csharp
public static explicit operator TargetType(SourceType source)
    => CastEngine.Cast<SourceType, TargetType>(source);
```

### Need bidirectional mapping?

**Solution:** Add cast operators in BOTH classes:
```csharp
// In UserEntity
public static explicit operator UserDto(UserEntity source)
    => CastEngine.Cast<UserEntity, UserDto>(source);

// In UserDto
public static explicit operator UserEntity(UserDto source)
    => CastEngine.Cast<UserDto, UserEntity>(source);
```

And configure BOTH directions:
```csharp
config.CreateMap<User, UserDto>().WithSync(/* ... */).Build();
config.CreateMap<UserDto, User>().WithSync(/* ... */).Build();
```

### Properties not mapping correctly?

**Solution:** Check your mapping configuration:
```csharp
config.CreateMap<User, UserDto>()
    .WithSync(user => new UserDto
    {
        Id = user.Id,              // Make sure all properties are mapped!
        FullName = $"{user.FirstName} {user.LastName}",
        Email = user.Email
    })
    .Build();
```
