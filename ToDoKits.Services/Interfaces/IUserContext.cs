namespace ToDoKits.Services.Interfaces;

/// <summary>当前登录用户上下文：从 JWT 的 ClaimsPrincipal 读取当前账号 Id。</summary>
public interface IUserContext
{
    /// <summary>当前登录用户 Id；未认证时为 0。</summary>
    long UserId { get; }
}
