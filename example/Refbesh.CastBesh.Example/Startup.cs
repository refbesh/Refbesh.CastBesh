using Refbesh.CastBesh.Example.Models;
using Refbesh.CastBesh;

namespace Refbesh.CastBesh.Example;

public static class Startup
{
    public static void ConfigureMappings()
    {
        CastBeshStartup.Configure(config =>
        {
            // UserEntity -> UserDto
            config.CreateMap<UserEntity, UserDto>()
                .WithSync(entity => new UserDto
                {
                    Id = entity.Id,
                    FullName = $"{entity.FirstName} {entity.LastName}",
                    Email = entity.Email,
                    Status = entity.IsActive ? "Active" : "Inactive",
                    Phone = entity.PhoneNumber,
                    LastLogin = entity.LastLoginDate?.ToString("yyyy-MM-dd HH:mm") ?? "Never"
                })
                .WithAsync(async (entity, ct) =>
                {
                    await Task.Delay(5, ct);
                    return new UserDto
                    {
                        Id = entity.Id,
                        FullName = $"{entity.FirstName} {entity.LastName}",
                        Email = entity.Email,
                        Status = entity.IsActive ? "Active" : "Inactive",
                        Phone = entity.PhoneNumber,
                        LastLogin = entity.LastLoginDate?.ToString("yyyy-MM-dd HH:mm") ?? "Never"
                    };
                })
                .Build();

            // UserEntity -> UserListDto
            config.CreateMap<UserEntity, UserListDto>()
                .WithSync(entity => new UserListDto
                {
                    Id = entity.Id,
                    Name = $"{entity.FirstName} {entity.LastName}",
                    Status = entity.IsActive ? "✓ Active" : "✗ Inactive"
                })
                .Build();

            // UserDto -> UserEntity (Reverse)
            config.CreateMap<UserDto, UserEntity>()
                .WithSync(dto =>
                {
                    var nameParts = dto.FullName.Split(' ', 2);
                    return new UserEntity
                    {
                        Id = dto.Id,
                        FirstName = nameParts.Length > 0 ? nameParts[0] : string.Empty,
                        LastName = nameParts.Length > 1 ? nameParts[1] : string.Empty,
                        Email = dto.Email,
                        IsActive = dto.Status == "Active",
                        PhoneNumber = dto.Phone,
                        CreatedAt = DateTime.UtcNow
                    };
                })
                .Build();

            // ProductEntity -> ProductDto
            config.CreateMap<ProductEntity, ProductDto>()
                .WithSync(entity => new ProductDto
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    DisplayPrice = $"${entity.Price:F2}",
                    InStock = entity.Stock > 0,
                    Category = entity.Category
                })
                .Build();

            // ProductEntity -> ProductDetailDto
            config.CreateMap<ProductEntity, ProductDetailDto>()
                .WithSync(entity => new ProductDetailDto
                {
                    Id = entity.Id,
                    Name = entity.Name,
                    Description = entity.Description,
                    DisplayPrice = $"${entity.Price:F2}",
                    AvailableQuantity = entity.Stock,
                    Availability = entity.Stock > 10 ? "In Stock" :
                                   entity.Stock > 0 ? "Low Stock" : "Out of Stock"
                })
                .Build();

            // ProductDto -> ProductEntity (Reverse)
            config.CreateMap<ProductDto, ProductEntity>()
                .WithSync(dto => new ProductEntity
                {
                    Id = dto.Id,
                    Name = dto.Name,
                    Price = decimal.Parse(dto.DisplayPrice.Replace("$", "").Replace(",", "")),
                    Stock = dto.InStock ? 1 : 0,
                    Category = dto.Category,
                    CreatedAt = DateTime.UtcNow
                })
                .Build();

            // OrderEntity -> OrderDto
            config.CreateMap<OrderEntity, OrderDto>()
                .WithSync(entity => new OrderDto
                {
                    Id = entity.Id,
                    UserId = entity.UserId,
                    OrderDate = entity.OrderDate.ToString("yyyy-MM-dd"),
                    TotalAmount = $"${entity.TotalAmount:F2}",
                    Status = entity.Status,
                    ItemCount = entity.Items.Count
                })
                .Build();
        });

        Console.WriteLine("✓ All mappings configured successfully!");
    }
}