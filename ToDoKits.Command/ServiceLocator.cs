using Autofac;

namespace ToDoKits.Command;

/// <summary>
/// 服务定位器：容器构建完成后初始化，允许任意位置「按属性/按类型」直接取实例，
/// 满足「容器提供能够直接通过属性获取实例，不用构造函数再赋值」的要求。
/// </summary>
public static class ServiceLocator
{
    private static IContainer? _container;

    /// <summary>容器构建完成后调用一次。</summary>
    public static void Initialize(IContainer container) => _container = container;

    /// <summary>按类型解析实例。</summary>
    public static T Resolve<T>() where T : notnull => _container!.Resolve<T>();

    /// <summary>按运行类型解析实例。</summary>
    public static object Resolve(Type type) => _container!.Resolve(type);
}
