using Autofac;

namespace ToDoKits.Command;

/// <summary>
/// 服务定位器：容器构建完成后初始化，允许任意位置「按属性/按类型」直接取实例，
/// 满足「容器提供能够直接通过属性获取实例，不用构造函数再赋值」的要求。
/// </summary>
public static class ServiceLocator
{
    private static ILifetimeScope? _scope;

    /// <summary>容器构建完成后调用一次（传入根作用域，它自我注册 ILifetimeScope，可稳定解析）。</summary>
    public static void Initialize(ILifetimeScope scope) => _scope = scope;

    /// <summary>按类型解析实例。</summary>
    public static T Resolve<T>() where T : notnull => _scope!.Resolve<T>();

    /// <summary>按运行类型解析实例。</summary>
    public static object Resolve(Type type) => _scope!.Resolve(type);
}
