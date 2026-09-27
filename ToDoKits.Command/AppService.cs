using Microsoft.Extensions.Logging;
using SqlSugar;

namespace ToDoKits.Command;

/// <summary>
/// 服务与控制器的基础类。所有依赖通过「属性」注入（由 Autofac 的
/// PropertiesAutowired 自动填充），无需再在构造函数里手动赋值。
/// </summary>
public abstract class AppService
{
    /// <summary>SqlSugar 数据库访问（PostgreSQL）。由 Autofac 属性注入。</summary>
    public ISqlSugarClient Db { get; set; } = null!;

    /// <summary>Serilog 日志工厂。由 Autofac 属性注入。</summary>
    public ILoggerFactory LogFactory { get; set; } = null!;

    private ILogger? _log;

    /// <summary>当前类型的日志器。</summary>
    protected ILogger Log => _log ??= LogFactory.CreateLogger(GetType());
}
