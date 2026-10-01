using SqlSugar;

namespace ToDoKits.Models.Entities;

/// <summary>好习惯实体（每日打卡）。</summary>
[SugarTable("habits")]
public class Habit
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }

    /// <summary>所属账号 Id（多租户隔离）。</summary>
    public long UserId { get; set; }

    [SugarColumn(Length = 100)]
    public string Name { get; set; } = "";

    /// <summary>所属类别（健康 / 学习…，独立于待办类别）。</summary>
    [SugarColumn(Length = 50)]
    public string Cat { get; set; } = "健康";

    /// <summary>目标描述，如「每天一次」。</summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Goal { get; set; }

    /// <summary>连续打卡天数。</summary>
    [SugarColumn]
    public int Streak { get; set; }

    /// <summary>提醒时间（HH:mm）。</summary>
    [SugarColumn(Length = 20, IsNullable = true)]
    public string? Time { get; set; }

    /// <summary>今天是否已打卡。</summary>
    [SugarColumn]
    public bool DoneToday { get; set; }

    /// <summary>上次打卡日期（yyyy-MM-dd），用于跨天自动重置 DoneToday 与连续天数。</summary>
    [SugarColumn(Length = 20, IsNullable = true)]
    public string? LastDoneDate { get; set; }
}
