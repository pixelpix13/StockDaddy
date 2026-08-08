using StockDaddy.Application.DTOs;

namespace StockDaddy.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<AuthResponse?> RegisterAsync(RegisterRequest request);
    Task<UserDto?> GetCurrentUserAsync(int userId);
    Task<AuthResponse?> RefreshSessionAsync(int userId);
    Task<AuthResponse?> SwitchStoreAsync(int userId, int storeId);
    Task<List<UserStoreOptionDto>> GetUserStoresAsync(int userId, int? activeStoreId = null);
}
