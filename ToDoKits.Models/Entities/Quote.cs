using SqlSugar;

namespace ToDoKits.Models.Entities;

/// <summary>名言警句实体。</summary>
[SugarTable("quotes")]
public class Quote
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }

    /// <summary>名言内容。</summary>
    [SugarColumn(Length = 1000)]
    public string Text { get; set; } = "";

    /// <summary>作者 / 说这话的人。</summary>
    [SugarColumn(Length = 100, IsNullable = true)]
    public string? Who { get; set; }

    /// <summary>出处，如《书名》。</summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Src { get; set; }

    /// <summary>标签（逗号分隔）。</summary>
    [SugarColumn(Length = 200, IsNullable = true)]
    public string? Tags { get; set; }

    /// <summary>记录日期（yyyy-MM-dd）。</summary>
    [SugarColumn(Length = 20)]
    public string Date { get; set; } = "";
}
