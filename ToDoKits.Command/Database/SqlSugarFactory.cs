using SqlSugar;

namespace ToDoKits.Command.Database;

/// <summary>
/// SqlSugar 客户端工厂：封装连接字符串并创建 PostgreSQL 访问实例。
/// </summary>
public static class SqlSugarFactory
{
    public static ISqlSugarClient Create(string connectionString)
    {
        var db = new SqlSugarClient(new ConnectionConfig
        {
            ConnectionString = connectionString,
            DbType = DbType.PostgreSQL,      // 数据库为 PostgreSQL
            IsAutoCloseConnection = true,
            InitKeyType = InitKeyType.Attribute,
            MoreSettings = new ConnMoreSettings { PgSqlIsAutoToLower = true }
        });
        return db;
    }
}
