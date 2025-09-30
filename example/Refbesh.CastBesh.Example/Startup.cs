using Refbesh.CastBesh.Example.Models;
using Refbesh.CastBesh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Refbesh.CastBesh.Example
{
    public class Startup
    {
        public static void ConfigureMappings()
        {
            CastBeshStartup.Configure(config =>
            {
                // Map UserEntity -> UserDto
                config.CreateMap<UserEntity, UserDto>()
                    .WithSync(entity => new UserDto
                    {
                        Id = entity.Id,
                        FullName = $"{entity.FirstName} {entity.LastName}",
                        Email = entity.Email,
                        Status = entity.IsActive ? "Active" : "Inactive"
                    })
                    .WithAsync(async (entity, ct) =>
                    {
                        await Task.Delay(10, ct); // Simulate async work
                        return new UserDto
                        {
                            Id = entity.Id,
                            FullName = $"{entity.FirstName} {entity.LastName}",
                            Email = entity.Email,
                            Status = entity.IsActive ? "Active" : "Inactive"
                        };
                    })
                    .Build();

                // Map UserDto -> UserEntity (reverse)
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
                            CreatedAt = DateTime.UtcNow
                        };
                    })
                    .Build();

                // Map ProductEntity -> ProductDto
                config.CreateMap<ProductEntity, ProductDto>()
                    .WithSync(entity => new ProductDto
                    {
                        Id = entity.Id,
                        Name = entity.Name,
                        DisplayPrice = $"${entity.Price:F2}",
                        InStock = entity.Stock > 0
                    })
                    .Build();

                // Map ProductDto -> ProductEntity (reverse)
                config.CreateMap<ProductDto, ProductEntity>()
                    .WithSync(dto => new ProductEntity
                    {
                        Id = dto.Id,
                        Name = dto.Name,
                        Price = decimal.Parse(dto.DisplayPrice.Replace("$", "")),
                        Stock = dto.InStock ? 1 : 0
                    })
                    .Build();
            });
        }
    }
}