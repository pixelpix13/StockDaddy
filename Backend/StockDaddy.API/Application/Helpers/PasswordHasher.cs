using System.Security.Cryptography;
using System.Text;

namespace StockDaddy.Application.Helpers;

public static class PasswordHasher
{
    public static string Hash(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    public static bool Verify(string password, string storedHash)
    {
        if (string.IsNullOrEmpty(storedHash))
        {
            return false;
        }

        if (password == storedHash)
        {
            return true;
        }

        return Hash(password) == storedHash;
    }
}
