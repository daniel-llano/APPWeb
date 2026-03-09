using System.Net.Http.Headers;
using System.Net.Http.Json;
using APPWeb.Application.DTOs;
using APPWeb.Application.Interfaces;
using APPWeb.Domain.Ports;

namespace APPWeb.Infrastructure.Http;

/// <summary>
/// Adaptador HTTP hacia la WBAPI (puerto de salida).
/// </summary>
public class ProductApiClient : IProductApiClient
{
    private readonly HttpClient _http;
    private readonly IAuthService _auth;

    public ProductApiClient(HttpClient http, IAuthService auth)
    {
        _http = http;
        _auth = auth;
    }

    private void AttachToken()
    {
        if (!string.IsNullOrEmpty(_auth.Token))
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _auth.Token);
    }

    public async Task<IReadOnlyList<ProductDto>> GetAllAsync(CancellationToken ct = default)
    {
        AttachToken();
        var result = await _http.GetFromJsonAsync<List<ProductDto>>("api/products", ct);
        return result?.AsReadOnly() ?? new List<ProductDto>().AsReadOnly();
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        AttachToken();
        return await _http.GetFromJsonAsync<ProductDto>($"api/products/{id}", ct);
    }

    public async Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken ct = default)
    {
        AttachToken();
        var response = await _http.PostAsJsonAsync("api/products", request, ct);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ProductDto>(cancellationToken: ct))!;
    }

    public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default)
    {
        AttachToken();
        var response = await _http.PutAsJsonAsync($"api/products/{id}", request, ct);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ProductDto>(cancellationToken: ct))!;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        AttachToken();
        var response = await _http.DeleteAsync($"api/products/{id}", ct);
        response.EnsureSuccessStatusCode();
    }
}
