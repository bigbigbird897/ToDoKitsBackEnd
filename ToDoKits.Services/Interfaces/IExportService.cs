namespace ToDoKits.Services.Interfaces;

/// <summary>导出服务：把各模块数据导出为 CSV（带 BOM，Excel 可直接打开）。</summary>
public interface IExportService
{
    /// <summary>导出一个模块：todo / habit / quote / note / diary。</summary>
    (string FileName, byte[] Bytes) ExportModule(string module);
    /// <summary>导出全部模块（文件名 → CSV 字节）。</summary>
    Dictionary<string, byte[]> ExportAll();
}
