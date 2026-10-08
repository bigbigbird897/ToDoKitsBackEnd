using SqlSugar;

namespace ToDoKits.Models.Entities;

/// <summary>工作笔记 · 文件夹实体（树结构，ParentId 指向父文件夹）。</summary>
[SugarTable("workfolders")]
public class WorkFolder
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }

    /// <summary>所属账号 Id（多租户隔离）。</summary>
    public long UserId { get; set; }

    [SugarColumn(Length = 200)]
    public string Name { get; set; } = "";

    /// <summary>父文件夹 Id；null 表示根目录。</summary>
    [SugarColumn(IsNullable = true)]
    public long? ParentId { get; set; }

    /// <summary>创建时间（yyyy-MM-dd HH:mm）。</summary>
    [SugarColumn(Length = 20)]
    public string CreatedAt { get; set; } = "";
}

/// <summary>工作笔记 · 文件实体（txt / md，内容存数据库）。</summary>
[SugarTable("workfiles")]
public class WorkFile
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public long Id { get; set; }

    /// <summary>所属账号 Id（多租户隔离）。</summary>
    public long UserId { get; set; }

    /// <summary>所属文件夹 Id；0 表示根目录。</summary>
    public long FolderId { get; set; }

    [SugarColumn(Length = 200)]
    public string Name { get; set; } = "";

    /// <summary>文件类型：txt / md。</summary>
    [SugarColumn(Length = 10)]
    public string Type { get; set; } = "txt";

    /// <summary>文件内容（text 类型，避免 varchar 长度上限）。</summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? Content { get; set; }

    /// <summary>最近修改时间（yyyy-MM-dd HH:mm）。</summary>
    [SugarColumn(Length = 20)]
    public string UpdatedAt { get; set; } = "";
}
