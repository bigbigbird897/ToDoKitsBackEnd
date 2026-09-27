using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Serilog;
using SqlSugar;
using ToDoKits.Command;
using ToDoKits.Command.Database;
using ToDoKits.Models;
using ToDoKits.Services.Implements;

// ===== Serilog 日志 =====
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/todokits-.log", rollingInterval: RollingInterval.Day, encoding: System.Text.Encoding.UTF8)
    .CreateLogger();

try
{
    Log.Information("ToDoKits 后端启动中…");
    var builder = WebApplication.CreateBuilder(args);

    // Serilog 接入 ASP.NET Core
    builder.Host.UseSerilog();

    // ===== Autofac 容器（属性注入，无需构造函数赋值）=====
    builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory(container =>
    {
        var conn = builder.Configuration.GetConnectionString("Default")
                   ?? "Host=localhost;Port=5432;Database=todokits;Username=postgres;Password=postgres";
        var loggerFactory = LoggerFactory.Create(lb => lb.AddSerilog());

        // 第三方组件：日志工厂 + SqlSugar(PostgreSQL)
        container.RegisterInstance(loggerFactory).As<ILoggerFactory>();
        container.Register(_ => SqlSugarFactory.Create(conn))
            .As<ISqlSugarClient>().SingleInstance();

        // 业务服务：接口 → 实现，全部注册 + 属性注入
        container.RegisterAssemblyTypes(typeof(TodoService).Assembly)
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Service"))
            .As(t => t.GetInterfaces().First(i => i.Name == "I" + t.Name))
            .InstancePerLifetimeScope()
            .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);

        // 控制器：属性注入（直接通过属性获取服务实例）
        container.RegisterAssemblyTypes(typeof(Program).Assembly)
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Controller"))
            .InstancePerLifetimeScope()
            .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies);
    }));

    builder.Services.AddControllers();
    builder.Services.AddCors(o => o.AddPolicy("any", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

    var app = builder.Build();

    // 初始化服务定位器
    var rootScope = app.Services.GetAutofacRoot();
    ServiceLocator.Initialize(rootScope.Resolve<IContainer>());

    // 初始化数据库（CodeFirst 建表，表已存在则自动补列）
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();
        DbInitializer.EnsureCreated(db);
        Log.Information("数据库表结构已就绪");
    }

    app.UseCors("any");
    app.MapControllers();

    // ===== 托管前端静态文件（FrontEnd/dist 存在时）=====
    var frontDist = Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "..", "FrontEnd", "dist"));
    if (Directory.Exists(frontDist))
    {
        var provider = new PhysicalFileProvider(frontDist);
        app.UseDefaultFiles(new DefaultFilesOptions { FileProvider = provider });
        app.UseStaticFiles(new StaticFileOptions { FileProvider = provider });
        app.MapFallbackToFile("index.html", new StaticFileOptions { FileProvider = provider });
        Log.Information("已托管前端静态资源：{Dist}", frontDist);
    }

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "后端异常退出");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
