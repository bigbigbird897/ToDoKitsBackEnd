using Microsoft.AspNetCore.Mvc;
using ToDoKits.Models.Dtos;
using ToDoKits.Services.Interfaces;

namespace ToDoKits.Controllers;

/// <summary>工作笔记接口：/api/worknotes（文件夹树 + 文件增删改查 / 内容读写）。</summary>
[ApiController]
[Route("api/worknotes")]
public class WorkNotesController : ControllerBase
{
    public IWorkNotesService WorkNotes { get; set; } = null!;

    // ===== 文件夹 =====
    [HttpGet("folders")]
    public async Task<IActionResult> GetFolders() => Ok(await WorkNotes.GetFoldersAsync());

    [HttpPost("folders")]
    public async Task<IActionResult> AddFolder([FromBody] WorkFolderInput req)
    {
        try { return Ok(await WorkNotes.AddFolderAsync(req.Name, req.ParentId)); }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("folders/{id:long}/rename")]
    public async Task<IActionResult> RenameFolder(long id, [FromBody] WorkFolderRename req)
    {
        try { return Ok(await WorkNotes.RenameFolderAsync(id, req.Name)); }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpDelete("folders/{id:long}")]
    public async Task<IActionResult> DeleteFolder(long id)
    {
        await WorkNotes.DeleteFolderAsync(id);
        return NoContent();
    }

    // ===== 文件 =====
    [HttpGet("files")]
    public async Task<IActionResult> GetFiles([FromQuery] long folderId = 0)
        => Ok(await WorkNotes.GetFilesAsync(folderId));

    [HttpGet("files/{id:long}")]
    public async Task<IActionResult> GetFile(long id)
    {
        try { return Ok(await WorkNotes.GetFileAsync(id)); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpGet("all-files")]
    public async Task<IActionResult> GetAllFiles() => Ok(await WorkNotes.GetAllFilesAsync());

    [HttpPost("files")]
    public async Task<IActionResult> CreateFile([FromBody] WorkFileInput input)
    {
        try { return Ok(await WorkNotes.CreateFileAsync(input)); }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("files/{id:long}")]
    public async Task<IActionResult> UpdateFile(long id, [FromBody] WorkFileInput input)
    {
        try { return Ok(await WorkNotes.UpdateFileAsync(id, input)); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpDelete("files/{id:long}")]
    public async Task<IActionResult> DeleteFile(long id)
    {
        await WorkNotes.DeleteFileAsync(id);
        return NoContent();
    }
}
