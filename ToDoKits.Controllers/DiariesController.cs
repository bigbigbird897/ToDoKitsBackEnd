using Microsoft.AspNetCore.Mvc;
using ToDoKits.Models.Dtos;
using ToDoKits.Services.Interfaces;

namespace ToDoKits.Controllers;

/// <summary>电子日记接口：/api/diaries</summary>
[ApiController]
[Route("api/diaries")]
public class DiariesController : ControllerBase
{
    public IDiaryService Diaries { get; set; } = null!;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await Diaries.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DiaryInput input)
        => Ok(await Diaries.CreateAsync(input));

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] DiaryInput input)
    {
        try { return Ok(await Diaries.UpdateAsync(id, input)); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await Diaries.DeleteAsync(id);
        return NoContent();
    }
}
