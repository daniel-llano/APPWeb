namespace APPWeb.Domain.Ports;

/// <summary>
/// Puerto de salida para el servicio de autenticación contra la API.
/// Estado reactivo: suscribe a StateChanged para forzar re-render en componentes.
/// </summary>
public interface IAuthService
{
    Task<string?> LoginAsync(string username, string password, CancellationToken ct = default);
    Task<string?> RegisterAsync(string username, string password, CancellationToken ct = default);
    void Logout();
    bool IsAuthenticated { get; }
    bool IsAdmin { get; }
    bool IsInvitado { get; }
    string? Token { get; }
    string? Role { get; }
    string? Username { get; }

    /// <summary>Fires whenever login, register, or logout changes auth state.</summary>
    event Action? StateChanged;
}
