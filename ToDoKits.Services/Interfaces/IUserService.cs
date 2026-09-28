using ToDoKits.Models.Dtos;
using ToDoKits.Models.Entities;

namespace ToDoKits.Services.Interfaces;

/// <summary>账号注册 / 登录服务。</summary>
public interface IUserService
{
    /// <summary>注册新账号（用户名唯一；密码加盐哈希后存储）。</summary>
    Task<User> RegisterAsync(RegisterInput input);

    /// <summary>校验用户名 + 密码，成功返回用户，失败返回 null。</summary>
    Task<User?> AuthenticateAsync(string username, string password);
}
