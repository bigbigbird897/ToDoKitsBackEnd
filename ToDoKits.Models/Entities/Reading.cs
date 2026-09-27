using SqlSugar;

namespace ToDoKits.Models.Entities;

/// <summary>读后感文件夹实体。</summary>
[SugarTable("folders")]
public class Folder
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }

    [SugarColumn(Length = 200)]
    public string Name { get; set; } = "";
}

/// <summary>读后感（笔记）实体，挂在某个文件夹下。</summary>
[SugarTable("notes")]
public class Note
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }

    /// <summary>所属文件夹名称。</summary>
    [SugarColumn(Length = 200)]
    public string Folder { get; set; } = "";

    [SugarColumn(Length = 200)]
    public string Title { get; set; } = "";

    [SugarColumn(Length = int.MaxValue, IsNullable = true)]
    public string? Content { get; set; }

    /// <summary>记录日期（yyyy-MM-dd）。</summary>
    [SugarColumn(Length = 20)]
    public string Date { get; set; } = "";
}
