using APPWeb.Application.DTOs;
using APPWeb.Domain.Entities;
using APPWeb.Domain.Ports;

namespace APPWeb.Application.Interfaces;

/// <summary>
/// Puerto de salida (driven port) — adaptador HTTP hacia la WBAPI.
/// </summary>
public interface IProductApiClient
{
    Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken ct = default);
    Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken ct = default);
    Task<ProductDto> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
