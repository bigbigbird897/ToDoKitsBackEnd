# ToDoKits 后端（BackEnd）

个人生活助手后端服务，基于 **.NET 10 + ASP.NET Core Web API**，**PostgreSQL + SqlSugar ORM**，**Autofac 属性注入**，**Serilog 日志**。

## 解决方案结构

```
ToDoKits.Backend.slnx
├── ToDoKits.Command/       # 依赖基座：所有其他项目依赖它
│   ├── AppService.cs       #   服务/控制器基类（Db、Log 属性注入）
│   ├── ServiceLocator.cs   #   服务定位器（按类型直接取实例）
│   ├── Database/SqlSugarFactory.cs
│   └── Helpers/CsvExporter.cs
├── ToDoKits.Models/        # PostgreSQL 实体 + DTO + 建表
│   ├── Entities/           #   Todo/TodoCategory/HabitCategory/Habit/Quote/Folder/Note/Diary
│   ├── Dtos/               #   TodoInput·HabitInput·QuoteInput·NoteInput·DiaryInput·RangeStatsResult·NameRequest
│   └── DbInitializer.cs
├── ToDoKits.Services/      # 业务层
│   ├── Interfaces/         #   ITodoService·ICategoryService·IHabitService·IQuoteService·IReadingService·IDiaryService·IStatsService·IExportService
│   └── Implements/         #   各服务实现（继承 AppService，Db 属性注入）
└── ToDoKits.Controllers/   # Web API 入口（对外接口）
    ├── Program.cs          #   Serilog + Autofac(属性注入) + SqlSugar + 静态托管
    ├── appsettings.json    #   PostgreSQL 连接串
    └── Todos·Categories·Habits·Quotes·Reading·Diaries·Stats·Export 控制器
```

## 依赖注入（Autofac 属性注入）

- 服务/控制器**通过属性直接获取实例，无需构造函数赋值**（`PropertiesAutowired`）。
- `Program.cs` 将第三方组件（`ISqlSugarClient`、`ILoggerFactory`）、全部业务服务（接口→实现）、全部控制器注册进 Autofac 容器，并对每个注册启用属性注入。
- `AppService` 基类提供 `Db`（SqlSugar）与 `Log`（Serilog）属性，继承即自动获得数据库与日志。

## 数据库

- PostgreSQL，连接串配置于 `appsettings.json` → `ConnectionStrings:Default`（可用环境变量 `ConnectionStrings__Default` 覆盖）。
- 启动时 `DbInitializer.EnsureCreated` 按实体 **CodeFirst 自动建表**（已存在自动补列）。
- 实体表：`todos` · `todo_categories` · `habit_categories` · `habits` · `quotes` · `folders` · `notes` · `diaries`。

## 对外接口（Controller）

| 模块 | 路径 |
|---|---|
| 待办 | `GET/POST /api/todos`、`GET/PUT/DELETE /api/todos/{id}`、`PUT /api/todos/{id}/toggle` |
| 分类 | `GET/POST/DELETE /api/categories/{todo\|habit}` |
| 习惯 | `GET/POST /api/habits`、`PUT/DELETE /api/habits/{id}`、`PUT /api/habits/{id}/toggle` |
| 名言 | `GET/POST /api/quotes`、`PUT/DELETE /api/quotes/{id}` |
| 读后感 | `GET/POST /api/reading/folders`、`DELETE /api/reading/folders/{name}`；`GET/POST /api/reading/notes`、`PUT/DELETE /api/reading/notes/{id}` |
| 日记 | `GET/POST /api/diaries`、`PUT/DELETE /api/diaries/{id}` |
| 统计 | `GET /api/stats/range?start=&end=` |
| 导出 | `GET /api/export/module/{module}`、`GET /api/export/all`（zip） |

## 运行

```bash
# 1. 准备 PostgreSQL，创建数据库 todokits
# 2. 配置 appsettings.json 连接串（或环境变量）
cd BackEnd
dotnet restore
dotnet build ToDoKits.Backend.slnx
dotnet run --project ToDoKits.Controllers   # http://localhost:5000
```

## 变更记录

- 2026-09-27：四项目解决方案创建；实体/DTO/建表；服务接口与实现；8 个 Controller + Program（Serilog + Autofac 属性注入 + SqlSugar）；编译 0 警告 0 错误。
- 2026-09-28：修复启动期 Autofac 异常（`IContainer has not been registered`）——`ServiceLocator` 改用 `ILifetimeScope` 存储/解析，`Program.cs` 直接传入 `GetAutofacRoot()` 根作用域，不再 `Resolve<IContainer>()`；编译 0 警告 0 错误。
- 2026-09-28：修复 Todo 写入报 `column Name does not exist`——`DbInitializer` 改为在建表前校验已存在表的列结构，列不齐（旧/大小写不一致的残留表）则先删表再重建；特性经反射读取避免强类型依赖；编译 0 警告 0 错误。
- 2026-09-28：修复建表报 `varchar length cannot exceed 10485760`——`Note.Content`、`Diary.Text` 两处长文本列由 `Length=int.MaxValue` 改为 `ColumnDataType="text"`（PostgreSQL text 无长度上限）；编译 0 警告 0 错误。
- 2026-09-28：修复控制器属性注入不生效（`AddControllersAsServices`，让 MVC 从 Autofac 容器解析控制器）；修复 PostgreSQL 大小写不一致（`PgSqlIsAutoToLower=true`，查询与建表统一小写）；修复并发查询 `Connection already open`（`ISqlSugarClient` 由 `SingleInstance` 改 `InstancePerLifetimeScope`，每请求一实例）；编译 0 警告 0 错误，接口 CRUD 实测通过。
