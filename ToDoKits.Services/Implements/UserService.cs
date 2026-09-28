using System.Security.Cryptography;
using System.Text;
using ToDoKits.Command;
using ToDoKits.Models.Dtos;
using ToDoKits.Models.Entities;
using ToDoKits.Services.Interfaces;

namespace ToDoKits.Services.Implements;

public class UserService : AppService, IUserService
{
    public async Task<User> RegisterAsync(RegisterInput input)
    {
        var username = (input.Username ?? "").Trim();
        if (username.Length < 2) throw new ArgumentException("用户名至少 2 个字符");
        if ((input.Password ?? "").Length < 4) throw new ArgumentException("密码至少 4 位");

        if (await Db.Queryable<User>().AnyAsync(u => u.Username == username))
            throw new InvalidOperationException($"用户名「{username}」已被注册");

        var (hash, salt) = HashPassword(input.Password ?? "");
        var user = new User
        {
            Username = username,
            PasswordHash = hash,
            Salt = salt,
            CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };
        var id = await Db.Insertable(user).ExecuteReturnIdentityAsync();
        user.Id = id;
        return user;
    }

    public async Task<User?> AuthenticateAsync(string username, string password)
    {
        var user = await Db.Queryable<User>().FirstAsync(u => u.Username == (username ?? "").Trim());
        if (user == null) return null;
        return VerifyPassword(password ?? "", user.Salt, user.PasswordHash) ? user : null;
    }

    // ---- PBKDF2 加盐哈希 ----
    private static (string Hash, string Salt) HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return (Convert.ToHexString(hash), Convert.ToBase64String(salt));
    }

    private static bool VerifyPassword(string password, string salt, string expectedHash)
    {
        try
        {
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, Convert.FromBase64String(salt), 100_000, HashAlgorithmName.SHA256, 32);
            return CryptographicOperations.FixedTimeEquals(hash, Convert.FromHexString(expectedHash));
        }
        catch
        {
            return false;
        }
    }
}
