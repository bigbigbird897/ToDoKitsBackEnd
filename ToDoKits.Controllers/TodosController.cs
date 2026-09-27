using Microsoft.AspNetCore.Mvc;
using ToDoKits.Models.Dtos;
using ToDoKits.Services.Interfaces;

namespace ToDoKits.Controllers;

/// <summary>待办事项接口：/api/todos</summary>
[ApiController]
[Route("api/todos")]
public class TodosController : ControllerBase
{
    // Autofac 属性注入：无需构造函数赋值
    public ITodoService Todos { get; set; } = null!;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await Todos.GetAllAsync());

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Get(long id)
    {
        var todo = await Todos.GetAsync(id);
        return todo == null ? NotFound() : Ok(todo);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TodoInput input)
        => Ok(await Todos.CreateAsync(input));

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] TodoInput input)
    {
        try { return Ok(await Todos.UpdateAsync(id, input)); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpPut("{id:long}/toggle")]
    public async Task<IActionResult> Toggle(long id)
    {
        var todo = await Todos.ToggleAsync(id);
        return todo == null ? NotFound() : Ok(todo);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await Todos.DeleteAsync(id);
        return NoContent();
    }
}
