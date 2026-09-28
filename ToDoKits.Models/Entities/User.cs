using SqlSugar;

namespace ToDoKits.Models.Entities;

/// <summary>账号实体。每个账号登录后只看到自己名下的数据（多租户按 UserId 隔离）。</summary>
[SugarTable("users")]
public class User
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }

    /// <summary>登录用户名（唯一）。</summary>
    [SugarColumn(Length = 100)]
    public string Username { get; set; } = "";

    /// <summary>密码哈希（PBKDF2，十六进制）。</summary>
    [SugarColumn(Length = 200)]
    public string PasswordHash { get; set; } = "";

    /// <summary>密码哈希盐（随机，Base64）。</summary>
    [SugarColumn(Length = 100)]
    public string Salt { get; set; } = "";

    /// <summary>注册时间（yyyy-MM-dd HH:mm:ss）。</summary>
    [SugarColumn(Length = 30, IsNullable = true)]
    public string? CreatedAt { get; set; }
}
