using SqlSugar;
using ToDoKits.Models.Entities;

namespace ToDoKits.Models;

/// <summary>
/// 数据库初始化：按实体 CodeFirst 建表（表已存在则自动补列）。
/// </summary>
public static class DbInitializer
{
    public static void EnsureCreated(ISqlSugarClient db)
    {
        db.CodeFirst.InitTables(
            typeof(Todo),
            typeof(TodoCategory),
            typeof(HabitCategory),
            typeof(Habit),
            typeof(Quote),
            typeof(Folder),
            typeof(Note),
            typeof(Diary));
    }
}
