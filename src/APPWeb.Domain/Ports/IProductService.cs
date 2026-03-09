using APPWeb.Domain.Entities;

namespace APPWeb.Domain.Ports;

/// <summary>
/// Puerto de entrada (driving port) para operaciones CRUD de productos.
/// </summary>
public interface IProductService
{
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Product> CreateAsync(Product product, CancellationToken ct = default);
    Task<Product> UpdateAsync(Product product, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
