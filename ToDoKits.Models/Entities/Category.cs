using SqlSugar;

namespace ToDoKits.Models.Entities;

/// <summary>待办事项类别（可新增、可删除）。</summary>
[SugarTable("todo_categories")]
public class TodoCategory
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }

    /// <summary>所属账号 Id（多租户隔离）。</summary>
    public long UserId { get; set; }

    [SugarColumn(Length = 50)]
    public string Name { get; set; } = "";
}

/// <summary>好习惯类别（与待办类别分开维护）。</summary>
[SugarTable("habit_categories")]
public class HabitCategory
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }

    /// <summary>所属账号 Id（多租户隔离）。</summary>
    public long UserId { get; set; }

    [SugarColumn(Length = 50)]
    public string Name { get; set; } = "";
}
