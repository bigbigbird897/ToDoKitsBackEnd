using Microsoft.AspNetCore.Mvc;
using ToDoKits.Models.Dtos;
using ToDoKits.Services.Interfaces;

namespace ToDoKits.Controllers;

/// <summary>分类接口：/api/categories（待办与习惯分类分开维护）。</summary>
[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    public ICategoryService Categories { get; set; } = null!;

    [HttpGet("todo")]
    public async Task<IActionResult> GetTodoCats() => Ok(await Categories.GetTodoCatsAsync());

    [HttpPost("todo")]
    public async Task<IActionResult> AddTodoCat([FromBody] NameRequest req)
    {
        await Categories.AddTodoCatAsync(req.Name);
        return Ok();
    }

    [HttpDelete("todo/{name}")]
    public async Task<IActionResult> DeleteTodoCat(string name)
    {
        await Categories.DeleteTodoCatAsync(name);
        return NoContent();
    }

    [HttpGet("habit")]
    public async Task<IActionResult> GetHabitCats() => Ok(await Categories.GetHabitCatsAsync());

    [HttpPost("habit")]
    public async Task<IActionResult> AddHabitCat([FromBody] NameRequest req)
    {
        await Categories.AddHabitCatAsync(req.Name);
        return Ok();
    }

    [HttpDelete("habit/{name}")]
    public async Task<IActionResult> DeleteHabitCat(string name)
    {
        await Categories.DeleteHabitCatAsync(name);
        return NoContent();
    }
}
