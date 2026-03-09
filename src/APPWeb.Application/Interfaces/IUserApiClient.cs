using APPWeb.Application.DTOs;

namespace APPWeb.Application.Interfaces;

/// <summary>
/// Puerto de salida para operaciones de administración de usuarios HTTP.
/// </summary>
public interface IUserApiClient
{
    Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken ct = default);
    Task<UserDto> ChangeRoleAsync(Guid userId, string role, CancellationToken ct = default);
    Task DeleteAsync(Guid userId, CancellationToken ct = default);
}
