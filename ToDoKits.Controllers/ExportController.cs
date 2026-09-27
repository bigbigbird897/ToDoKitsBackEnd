using System.IO.Compression;
using Microsoft.AspNetCore.Mvc;
using ToDoKits.Services.Interfaces;

namespace ToDoKits.Controllers;

/// <summary>数据导出接口：/api/export。</summary>
[ApiController]
[Route("api/export")]
public class ExportController : ControllerBase
{
    public IExportService Export { get; set; } = null!;

    [HttpGet("module/{module}")]
    public IActionResult Module(string module)
    {
        var (fileName, bytes) = Export.ExportModule(module);
        return File(bytes, "text/csv; charset=utf-8", fileName);
    }

    [HttpGet("all")]
    public IActionResult All()
    {
        var all = Export.ExportAll();
        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
        {
            foreach (var (name, bytes) in all)
            {
                var entry = zip.CreateEntry(name, CompressionLevel.Fastest);
                using var es = entry.Open();
                es.Write(bytes);
            }
        }
        return File(ms.ToArray(), "application/zip", $"生活助手全量数据-{DateTime.Now:yyyyMMdd}.zip");
    }
}
