using SqlSugar;

namespace ToDoKits.Models.Entities;

/// <summary>待办事项实体。</summary>
[SugarTable("todos")]
public class Todo
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }

    /// <summary>所属账号 Id（多租户隔离）。</summary>
    public long UserId { get; set; }

    /// <summary>事项名称。</summary>
    [SugarColumn(Length = 200,IsNullable =true)]
    public string Name { get; set; } = "";

    /// <summary>所属类别（工作 / 个人 / 旅游…）。</summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string Cat { get; set; } = "工作";

    /// <summary>开始时间（默认创建时间，可改，yyyy-MM-dd）。</summary>
    [SugarColumn(Length = 20)]
    public string Start { get; set; } = "";

    /// <summary>预计完成时间（yyyy-MM-dd）。</summary>
    [SugarColumn(Length = 20, IsNullable = true)]
    public string? Due { get; set; }

    /// <summary>完成状态：doing / done。</summary>
    [SugarColumn(Length = 20)]
    public string Status { get; set; } = "doing";

    /// <summary>周期重复：'' / daily / weekly / monthly / yearly。</summary>
    [SugarColumn(Length = 20)]
    public string Repeat { get; set; } = "";

    /// <summary>备注。</summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Note { get; set; }

    /// <summary>实际完成时间（yyyy-MM-dd）。</summary>
    [SugarColumn(Length = 20, IsNullable = true)]
    public string? CompletedAt { get; set; }
}
