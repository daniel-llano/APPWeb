using APPWeb.Application.DTOs;
using APPWeb.Application.Interfaces;
using APPWeb.Domain.Ports;

namespace APPWeb.Infrastructure.Services;

/// <summary>
/// Servicio de autenticación — mantiene el token en memoria durante la sesión.
/// Implementa el patrón State para el estado de autenticación.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IAuthApiClient _apiClient;
    private string? _token;
    private string? _role;
    private string? _username;

    public event Action? StateChanged;

    public AuthService(IAuthApiClient apiClient) => _apiClient = apiClient;

    public bool IsAuthenticated => !string.IsNullOrEmpty(_token);
    public bool IsAdmin    => string.Equals(_role, "Admin",    StringComparison.OrdinalIgnoreCase);
    public bool IsInvitado => string.Equals(_role, "Invitado", StringComparison.OrdinalIgnoreCase);
    public string? Token    => _token;
    public string? Role     => _role;
    public string? Username => _username;

    public async Task<string?> LoginAsync(string username, string password, CancellationToken ct = default)
    {
        var response = await _apiClient.LoginAsync(new LoginRequest(username, password), ct);
        if (response is null) return null;
        SetSession(response);
        return _token;
    }

    public async Task<string?> RegisterAsync(string username, string password, CancellationToken ct = default)
    {
        var response = await _apiClient.RegisterAsync(new RegisterRequest(username, password), ct);
        if (response is null) return null;
        SetSession(response);
        return _token;
    }

    public void Logout()
    {
        _token    = null;
        _role     = null;
        _username = null;
        StateChanged?.Invoke();
    }

    private void SetSession(AuthResponse response)
    {
        _token    = response.AccessToken;
        _role     = response.Role;
        _username = response.Username;
        StateChanged?.Invoke();
    }
}
