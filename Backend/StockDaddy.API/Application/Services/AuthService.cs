using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using StockDaddy.Application.Authorization;
using StockDaddy.Application.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Domain.Entities;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Application.Services;

/// <summary>
/// Issues JWT tokens on login/register and validates credentials against Users table.
/// </summary>
public class AuthService : IAuthService
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly RbacService _rbacService;

    public AuthService(ApplicationDbContext context, IConfiguration configuration, RbacService rbacService)
    {
        _context = context;
        _configuration = configuration;
        _rbacService = rbacService;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var normalizedInput = request.UsernameOrEmail.Trim().ToLower();

        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => !u.IsDeleted && 
                (u.Email.ToLower() == normalizedInput || u.Username.ToLower() == normalizedInput));

        if (user == null)
        {
            return null;
        }

        if (!PasswordHasher.Verify(request.Password, user.PasswordHash))
        {
            return null;
        }

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<AuthResponse?> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await _context.Users
            .AnyAsync(u => !u.IsDeleted && 
                (u.Email.ToLower() == request.Email.Trim().ToLower() || u.Username.ToLower() == request.Username.Trim().ToLower()));

        if (existingUser)
        {
            return null;
        }

        var user = new User
        {
            TenantId = request.TenantId,
            RoleId = request.RoleId,
            StoreId = request.StoreId,
            Username = request.Username.Trim(),
            Email = request.Email.Trim(),
            PasswordHash = PasswordHasher.Hash(request.Password),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsDeleted = false
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var role = await _context.Roles.FindAsync(user.RoleId);
        user.Role = role;

        return await GenerateAuthResponseAsync(user);
    }

    public async Task<UserDto?> GetCurrentUserAsync(int userId)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);

        if (user == null)
        {
            return null;
        }

        var permissions = await _rbacService.GetPermissionKeysForRoleAsync(user.RoleId);

        return new UserDto
        {
            Id = user.Id,
            TenantId = user.TenantId,
            RoleId = user.RoleId,
            RoleName = user.Role?.Name ?? string.Empty,
            StoreId = user.StoreId,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IsDeleted = user.IsDeleted,
            DeletedAt = user.DeletedAt,
            Permissions = permissions
        };
    }

    private async Task<AuthResponse> GenerateAuthResponseAsync(User user)
    {
        var permissions = await _rbacService.GetPermissionKeysForRoleAsync(user.RoleId);

        var jwtSettings = _configuration.GetSection("Jwt");
        var secret = jwtSettings["Secret"] ?? "StockDaddy_Super_Secret_Key_For_JWT_Authentication_2026_Minimum_32_Chars!";
        var issuer = jwtSettings["Issuer"] ?? "StockDaddyAPI";
        var audience = jwtSettings["Audience"] ?? "StockDaddyClient";
        var expiryMinutes = double.TryParse(jwtSettings["ExpiryMinutes"], out var mins) ? mins : 1440;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new("tenantId", user.TenantId.ToString()),
            new("roleId", user.RoleId.ToString())
        };

        if (user.StoreId.HasValue)
        {
            claims.Add(new Claim("storeId", user.StoreId.Value.ToString()));
        }

        if (user.Role != null)
        {
            claims.Add(new Claim(ClaimTypes.Role, user.Role.Name));
        }

        foreach (var permission in permissions)
        {
            claims.Add(new Claim(PermissionKeys.ClaimType, permission));
        }

        var expiration = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiration,
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthResponse
        {
            Token = tokenString,
            Expiration = expiration,
            User = new UserDto
            {
                Id = user.Id,
                TenantId = user.TenantId,
                RoleId = user.RoleId,
                RoleName = user.Role?.Name ?? string.Empty,
                StoreId = user.StoreId,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                IsDeleted = user.IsDeleted,
                DeletedAt = user.DeletedAt,
                Permissions = permissions
            }
        };
    }
}
