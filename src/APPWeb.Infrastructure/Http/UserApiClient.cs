using System.Net.Http.Headers;
using System.Net.Http.Json;
using APPWeb.Application.DTOs;
using APPWeb.Application.Interfaces;
using APPWeb.Domain.Ports;

namespace APPWeb.Infrastructure.Http;

/// <summary>
/// Adaptador HTTP para gestión de usuarios contra la WBAPI (Admin only).
/// </summary>
public class UserApiClient : IUserApiClient
{
    private readonly HttpClient _http;
    private readonly IAuthService _auth;

    public UserApiClient(HttpClient http, IAuthService auth)
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

    public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken ct = default)
    {
        AttachToken();
        var result = await _http.GetFromJsonAsync<List<UserDto>>("api/users", ct);
        return result?.AsReadOnly() ?? new List<UserDto>().AsReadOnly();
    }

    public async Task<UserDto> ChangeRoleAsync(Guid userId, string role, CancellationToken ct = default)
    {
        AttachToken();
        var response = await _http.PutAsJsonAsync($"api/users/{userId}/role", new ChangeRoleRequest(role), ct);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<UserDto>(cancellationToken: ct))!;
    }

    public async Task DeleteAsync(Guid userId, CancellationToken ct = default)
    {
        AttachToken();
        var response = await _http.DeleteAsync($"api/users/{userId}", ct);
        response.EnsureSuccessStatusCode();
    }
}
