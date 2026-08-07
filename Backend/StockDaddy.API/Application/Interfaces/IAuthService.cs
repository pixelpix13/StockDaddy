using StockDaddy.Application.DTOs;

namespace StockDaddy.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<AuthResponse?> RegisterAsync(RegisterRequest request);
    Task<UserDto?> GetCurrentUserAsync(int userId);
}
