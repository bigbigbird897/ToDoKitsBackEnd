namespace ToDoKits.Models.Dtos;

/// <summary>仅含名称的请求（新增分类 / 文件夹）。</summary>
public class NameRequest
{
    public string Name { get; set; } = "";
}
