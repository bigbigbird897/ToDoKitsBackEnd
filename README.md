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

## Swagger 接口调试（Debug 环境）

- 在 **Development（Debug）** 环境下启动，接口自动展示在 `http://localhost:<port>/swagger`（默认 5000；若 `appsettings.json` 配置了 Kestrel 端口则以配置端口为准，如 9610）。
- 发布 / 生产环境**不启用** Swagger（`Program.cs` 中 `app.Environment.IsDevelopment()` 判断）。
- Swagger 支持 **JWT Bearer 调试**：先用 `POST /api/auth/register`（或 `/login`）拿到 token，点页面右上角 **Authorize**，粘贴 token 即可调试带锁图标（需登录）的接口。
- 依赖：`Swashbuckle.AspNetCore` 包（`AddEndpointsApiExplorer` + `AddSwaggerGen` 注册，`UseSwagger` + `UseSwaggerUI` 中间件）。

## 发布与 GitHub Actions 打包

三个仓库（父仓库管理 2 个子模块）已关联 GitHub：

| 仓库 | 地址 |
|---|---|
| 父仓库 | `git@github.com:bigbigbird897/ToDoKits.git` |
| 后端 | `git@github.com:bigbigbird897/ToDoKitsBackEnd.git` |
| 前端 | `git@github.com:bigbigbird897/ToDoKitsFrontEnd.git` |

发布步骤（SSH 认证，网络瞬断时报 `ssh: connect to host github.com port 22 ...` / `correct access rights` 多为偶发，重试即可）：

```bash
# 1. 配置远端
git -C FrontEnd remote add origin git@github.com:bigbigbird897/ToDoKitsFrontEnd.git
git -C BackEnd  remote add origin git@github.com:bigbigbird897/ToDoKitsBackEnd.git
git -C ToDoKits remote add origin git@github.com:bigbigbird897/ToDoKits.git

# 2. .gitmodules 中子模块 URL 指向远端 https（Actions checkout 需要，不能是本地相对路径）
#    FrontEnd -> https://github.com/bigbigbird897/ToDoKitsFrontEnd.git
#    BackEnd  -> https://github.com/bigbigbird897/ToDoKitsBackEnd.git

# 3. 打 tag 并推送（先后端 / 前端，再父仓库）
git -C FrontEnd tag v1.0.0 && git -C FrontEnd push -u origin main && git -C FrontEnd push origin v1.0.0
git -C BackEnd  tag v1.0.0 && git -C BackEnd  push -u origin main && git -C BackEnd  push origin v1.0.0
git -C ToDoKits tag v1.0.0 && git -C ToDoKits push -u origin main && git -C ToDoKits push origin v1.0.0
```

GitHub Actions（父仓库 `.github/workflows/release.yml`）：
- 触发：向父仓库推送 `v*` 标签（如 `v1.0.0`），或 `workflow_dispatch` 手动触发。
- 流程：`actions/checkout`（含子模块）→ 前端 `npm ci && npm run build`；后端 `dotnet publish`（linux-x64 / win-x64 自包含，非单文件）→ 组装 win/linux 单包（后端可执行 + 前端 dist 进 `wwwroot`）+ 纯前端静态产物 → `softprops/action-gh-release` 生成 GitHub Release。
- 产物：`ToDoKits-win-x64.zip`、`ToDoKits-linux-x64.zip`、`ToDoKits-frontend-www.zip`。

注意：
- 若子模块（如 ToDoKitsBackEnd）为**私有**仓库，Actions 的 `GITHUB_TOKEN` 无法跨仓库拉取私有子模块，会导致 `checkout` 失败——需将子模块设为 public，或在仓库 `Settings → Secrets and variables → Actions` 配置可访问子模块的 PAT，并在 workflow 的 `checkout` 步骤通过 `token` 传入。

**打包版本锁定（`release-tags.json`）**：
- 父仓库根目录 `release-tags.json` 记录当前打包所用的前端/后端 tag，例如 `{"FrontEnd":"v1.0.1","BackEnd":"v1.0.1"}`。
- `release.yml` 的 `build-frontend` / `build-backend` 在 `checkout`（含子模块）后，会读取该文件，把对应子模块 `git fetch --tags && git checkout -f <tag>` 到指定 tag 再构建，保证打包内容与父仓库 gitlink 指针解耦、严格锁定到配置版本。
- 更换打包版本：给子模块打新 tag 并推送 → 更新 `release-tags.json` → 父仓库提交推送并打新 `v*` tag 触发。

## 变更记录

- 2026-09-28：发布 v1.0.0 到 GitHub——父仓库（ToDoKits）+ 子模块 FrontEnd/BackEnd 配置 SSH 远端；`.gitmodules` 子模块 URL 由本地相对路径改为远端 https；三个仓库各自打 `v1.0.0` tag 并推送 main 与 tag；父仓库 `v1.0.0` 推送触发 GitHub Actions `release.yml` 打包 win/linux 单包与前端产物生成 Release。
- 2026-09-28：修复 GitHub Actions 打包——`download-artifact` 去掉 `merge-multiple`（保留各产物独立子目录，修复 `cp artifacts/backend-win-x64/*` 找不到目录）；关闭 `generate_release_notes` 与改用 `GH_TOKEN || github.token`（规避 GITHUB_TOKEN 对 generate-notes / create-release 接口的 403，仓库开启 Actions Read and write 权限后创建 Release 成功）；新增 `release-tags.json` 锁定打包所用的前端/后端 tag，`release.yml` 构建 job 先切换到配置 tag 再打包。
- 2026-09-28：前后端升级发布 v1.0.1——为 FrontEnd（`7ea0084`）与 BackEnd（`31f6e09`）打 `v1.0.1` tag 并推送；更新 `release-tags.json` 为 `{"FrontEnd":"v1.0.1","BackEnd":"v1.0.1"}`；父仓库打 `v1.0.5` tag 触发重新打包。
- 2026-09-27：四项目解决方案创建；实体/DTO/建表；服务接口与实现；8 个 Controller + Program（Serilog + Autofac 属性注入 + SqlSugar）；编译 0 警告 0 错误。
- 2026-09-28：修复启动期 Autofac 异常（`IContainer has not been registered`）——`ServiceLocator` 改用 `ILifetimeScope` 存储/解析，`Program.cs` 直接传入 `GetAutofacRoot()` 根作用域，不再 `Resolve<IContainer>()`；编译 0 警告 0 错误。
- 2026-09-28：修复 Todo 写入报 `column Name does not exist`——`DbInitializer` 改为在建表前校验已存在表的列结构，列不齐（旧/大小写不一致的残留表）则先删表再重建；特性经反射读取避免强类型依赖；编译 0 警告 0 错误。
- 2026-09-28：修复建表报 `varchar length cannot exceed 10485760`——`Note.Content`、`Diary.Text` 两处长文本列由 `Length=int.MaxValue` 改为 `ColumnDataType="text"`（PostgreSQL text 无长度上限）；编译 0 警告 0 错误。
- 2026-09-28：修复控制器属性注入不生效（`AddControllersAsServices`，让 MVC 从 Autofac 容器解析控制器）；修复 PostgreSQL 大小写不一致（`PgSqlIsAutoToLower=true`，查询与建表统一小写）；修复并发查询 `Connection already open`（`ISqlSugarClient` 由 `SingleInstance` 改 `InstancePerLifetimeScope`，每请求一实例）；编译 0 警告 0 错误，接口 CRUD 实测通过。
- 2026-09-28：新增账号体系（JWT 认证 + 多租户数据隔离）——`User` 实体+users 表、`/api/auth/register` `/api/auth/login`（PBKDF2 加盐哈希，签发 HS256 JWT）；全局 `[Authorize]`，所有数据表加 `UserId` 列并按当前账号过滤（待办/习惯/名言/读后感/日记/分类/统计/导出全隔离）；`IUserContext` 从 JWT 读当前用户；DbInitializer 增加「InitTables 失败（如新增 NOT NULL 列遇存量行）则删除重建」兜底。端到端实测：401、登录、注册、账号间数据隔离、错误密码 401、重复注册 409 全部通过。
- 2026-09-30：添加 Swagger 组件（Swashbuckle.AspNetCore 8.1.0）——Development 环境启用，接口展示在 `/swagger`；配置 JWT Bearer 安全定义（右上角 Authorize 填 token 调试带认证接口）；移除未使用的 `Microsoft.AspNetCore.OpenApi`（其依赖 Microsoft.OpenApi 2.x 与 Swashbuckle 8 的 1.6 API 冲突致编译失败）。编译 0 警告 0 错误，Swagger 页面与 `/swagger/v1/swagger.json` 实测正常。
