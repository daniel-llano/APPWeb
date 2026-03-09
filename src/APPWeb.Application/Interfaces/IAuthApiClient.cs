using APPWeb.Application.DTOs;

namespace APPWeb.Application.Interfaces;

/// <summary>
/// Puerto de salida para operaciones de autenticación HTTP.
/// </summary>
public interface IAuthApiClient
{
    Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<AuthResponse?> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
}
