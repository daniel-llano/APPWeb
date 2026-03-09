using APPWeb.Application.Interfaces;
using APPWeb.Domain.Entities;
using APPWeb.Domain.Ports;

namespace APPWeb.Application.Services;

/// <summary>
/// Servicio de aplicación — orquesta llamadas al adaptador HTTP (patrón Fachada/Service).
/// </summary>
public class ProductAppService : IProductService
{
    private readonly IProductApiClient _client;

    public ProductAppService(IProductApiClient client) => _client = client;

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default)
    {
        var dtos = await _client.GetAllAsync(ct);
        return dtos.Select(MapToDomain).ToList().AsReadOnly();
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var dto = await _client.GetByIdAsync(id, ct);
        return dto is null ? null : MapToDomain(dto);
    }

    public async Task<Product> CreateAsync(Product product, CancellationToken ct = default)
    {
        var dto = await _client.CreateAsync(
            new(product.Name, product.Description, product.Price, product.Category, product.Stock), ct);
        return MapToDomain(dto);
    }

    public async Task<Product> UpdateAsync(Product product, CancellationToken ct = default)
    {
        var dto = await _client.UpdateAsync(product.Id,
            new(product.Name, product.Description, product.Price, product.Category, product.Stock), ct);
        return MapToDomain(dto);
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
        => _client.DeleteAsync(id, ct);

    private static Product MapToDomain(APPWeb.Application.DTOs.ProductDto dto) => new()
    {
        Id = dto.Id, Name = dto.Name, Description = dto.Description,
        Price = dto.Price, Category = dto.Category, Stock = dto.Stock,
        CreatedAt = dto.CreatedAt, UpdatedAt = dto.UpdatedAt
    };
}
