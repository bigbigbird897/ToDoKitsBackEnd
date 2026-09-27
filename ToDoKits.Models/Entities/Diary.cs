using SqlSugar;

namespace ToDoKits.Models.Entities;

/// <summary>电子日记实体。</summary>
[SugarTable("diaries")]
public class Diary
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }

    /// <summary>日期（yyyy-MM-dd）。</summary>
    [SugarColumn(Length = 20)]
    public string Date { get; set; } = "";

    /// <summary>今天是星期几，如「星期一」。</summary>
    [SugarColumn(Length = 20, IsNullable = true)]
    public string? Weekday { get; set; }

    /// <summary>记录地点（城市）。</summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Location { get; set; }

    /// <summary>天气，如「晴 / 多云 / 小雨」。</summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Weather { get; set; }

    /// <summary>日记正文。</summary>
    [SugarColumn(Length = int.MaxValue, IsNullable = true)]
    public string? Text { get; set; }
}
