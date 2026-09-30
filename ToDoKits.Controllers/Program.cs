using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ToDoKits.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Serilog;
using SqlSugar;
using ToDoKits.Command;
using ToDoKits.Command.Database;
using ToDoKits.Models;
using ToDoKits.Services.Implements;
using Microsoft.OpenApi.Models;

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
                   ?? "Host=localhost;Port=5432;Database=todokits;Username=postgres;Password=11";
        var loggerFactory = LoggerFactory.Create(lb => lb.AddSerilog());

        // 第三方组件：日志工厂 + SqlSugar(PostgreSQL)
        container.RegisterInstance(loggerFactory).As<ILoggerFactory>();
        // InstancePerLifetimeScope：每个请求一个客户端，避免并发查询共享单实例导致 Connection already open
        container.Register(_ => SqlSugarFactory.Create(conn))
            .As<ISqlSugarClient>().InstancePerLifetimeScope();

        // 当前登录用户上下文（从 JWT 的 ClaimsPrincipal 读取 UserId）
        container.RegisterType<ToDoKits.Controllers.UserContext>().As<IUserContext>().InstancePerLifetimeScope();

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

    // AddControllersAsServices：让 MVC 从 Autofac 容器解析控制器，
    // 使控制器上的 PropertiesAutowired 属性注入生效。
    // AddControllersAsServices：让 MVC 从 Autofac 容器解析控制器，
    // 使控制器上的 PropertiesAutowired 属性注入生效；并全局要求登录（AuthController 已 AllowAnonymous）。
    builder.Services.AddControllers(o =>
    {
        var policy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
        o.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter(policy));
    }).AddControllersAsServices();
    builder.Services.AddCors(o => o.AddPolicy("any", p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

    // ===== JWT 认证：登录后签发 token，前端带 Authorization: Bearer <token> =====
    builder.Services.AddHttpContextAccessor();
    var jwtKey = builder.Configuration["Jwt:Secret"] ?? "ToDoKits-Dev-Key-Change-Me-2026!!";
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "todokits",
                ValidateAudience = true,
                ValidAudience = "todokits",
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(2)
            };
        });

    // ===== Swagger：Debug(Development) 环境把接口显示到 /swagger，支持 JWT Bearer 调试 =====
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(o =>
    {
        o.SwaggerDoc("v1", new OpenApiInfo { Title = "ToDoKits API", Version = "v1", Description = "个人生活助手后端接口（调试用）" });
        // 让 Swagger 右上角出现 Authorize 按钮，可粘贴登录后返回的 JWT token 调试带认证接口
        o.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "登录后返回的 token（直接粘贴 token 即可，Swagger 会自动加 Bearer 前缀）",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });
        o.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, Array.Empty<string>() }
        });
    });

    var app = builder.Build();

    // 初始化服务定位器（根作用域自我注册 ILifetimeScope，直接用，勿再 Resolve<IContainer>）
    var rootScope = app.Services.GetAutofacRoot();
    ServiceLocator.Initialize(rootScope);

    // 初始化数据库（CodeFirst 建表，表已存在则自动补列）
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();
        DbInitializer.EnsureCreated(db);
        Log.Information("数据库表结构已就绪");
    }

    app.UseCors("any");

    // Swagger 仅 Development（Debug）环境启用，发布/生产环境不暴露接口文档
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(o => o.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDoKits API v1"));
        Log.Information("Swagger 已启用：http://localhost:5000/swagger");
    }

    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    // 托管 wwwroot（发布时把前端 dist 拷贝到此即成为单包可运行版本）
    app.UseDefaultFiles();
    app.UseStaticFiles();

    // ===== 开发便捷：直接托管仓库内 FrontEnd/dist（存在时）=====
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
