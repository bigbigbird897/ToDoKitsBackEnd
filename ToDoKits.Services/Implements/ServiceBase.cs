using ToDoKits.Command;
using ToDoKits.Services.Interfaces;

namespace ToDoKits.Services.Implements;

/// <summary>
/// 业务服务基类：在 AppService（Db/日志）基础上，额外注入当前登录用户上下文。
/// 所有业务服务继承本类，即可用 <see cref="User"/> 按账号隔离数据。
/// </summary>
public abstract class ServiceBase : AppService
{
    /// <summary>当前登录用户上下文（由 Autofac 属性注入，从 JWT 读取 UserId）。</summary>
    public IUserContext User { get; set; } = null!;
}
