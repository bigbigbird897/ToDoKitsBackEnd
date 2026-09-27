using Microsoft.AspNetCore.Mvc;
using ToDoKits.Models.Dtos;
using ToDoKits.Services.Interfaces;

namespace ToDoKits.Controllers;

/// <summary>读后感接口：/api/reading（文件夹 + 笔记）。</summary>
[ApiController]
[Route("api/reading")]
public class ReadingController : ControllerBase
{
    public IReadingService Reading { get; set; } = null!;

    [HttpGet("folders")]
    public async Task<IActionResult> GetFolders() => Ok(await Reading.GetFoldersAsync());

    [HttpPost("folders")]
    public async Task<IActionResult> AddFolder([FromBody] NameRequest req)
    {
        await Reading.AddFolderAsync(req.Name);
        return Ok();
    }

    [HttpDelete("folders/{name}")]
    public async Task<IActionResult> DeleteFolder(string name)
    {
        await Reading.DeleteFolderAsync(name);
        return NoContent();
    }

    [HttpGet("notes")]
    public async Task<IActionResult> GetNotes() => Ok(await Reading.GetNotesAsync());

    [HttpPost("notes")]
    public async Task<IActionResult> CreateNote([FromBody] NoteInput input)
        => Ok(await Reading.CreateNoteAsync(input));

    [HttpPut("notes/{id:long}")]
    public async Task<IActionResult> UpdateNote(long id, [FromBody] NoteInput input)
    {
        try { return Ok(await Reading.UpdateNoteAsync(id, input)); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpDelete("notes/{id:long}")]
    public async Task<IActionResult> DeleteNote(long id)
    {
        await Reading.DeleteNoteAsync(id);
        return NoContent();
    }
}
