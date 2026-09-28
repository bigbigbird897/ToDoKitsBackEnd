using System.Reflection;
using SqlSugar;
using ToDoKits.Models.Entities;

namespace ToDoKits.Models;

/// <summary>
/// 数据库初始化：按实体 CodeFirst 建表。
/// 若某张表已存在但列结构与实体不一致（旧版本 / 不同大小写残留），
/// 则先删除该表再重建，避免“列不存在”类错误；列结构一致的正常表不会重建，数据保留。
/// 特性通过反射读取，避免对 SqlSugar 特性类型的强类型依赖。
/// </summary>
public static class DbInitializer
{
    private static readonly Type[] EntityTypes =
    {
        typeof(Todo), typeof(TodoCategory), typeof(HabitCategory),
        typeof(Habit), typeof(Quote), typeof(Folder), typeof(Note), typeof(Diary),
    };

    public static void EnsureCreated(ISqlSugarClient db)
    {
        foreach (var t in EntityTypes)
        {
            var tableName = GetTableName(t);
            if (db.DbMaintenance.IsAnyTable(tableName, false))
            {
                // 已存在的表：校验列是否齐全；缺失说明 schema 不兼容 → 重建
                var existing = db.DbMaintenance
                    .GetColumnInfosByTableName(tableName, false)
                    .Select(c => c.DbColumnName)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                var need = t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Select(GetColumnName);

                if (!need.All(existing.Contains))
                {
                    db.DbMaintenance.DropTable(tableName);
                }
            }

            db.CodeFirst.InitTables(t);
        }
    }

    private static string GetTableName(Type t)
    {
        var attr = t.GetCustomAttributes().FirstOrDefault(a => a.GetType().Name == "SugarTableAttribute");
        if (attr != null)
        {
            var p = attr.GetType().GetProperty("TableName");
            if (p?.GetValue(attr) is string s && !string.IsNullOrEmpty(s)) return s;
        }
        return t.Name;
    }

    private static string GetColumnName(PropertyInfo p)
    {
        var attr = p.GetCustomAttributes().FirstOrDefault(a => a.GetType().Name == "SugarColumnAttribute");
        if (attr != null)
        {
            var prop = attr.GetType().GetProperty("ColumnName");
            if (prop?.GetValue(attr) is string s && !string.IsNullOrEmpty(s)) return s;
        }
        return p.Name;
    }
}
