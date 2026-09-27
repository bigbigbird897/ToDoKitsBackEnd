using Microsoft.AspNetCore.Mvc;
using ToDoKits.Models.Dtos;
using ToDoKits.Services.Interfaces;

namespace ToDoKits.Controllers;

/// <summary>好习惯接口：/api/habits</summary>
[ApiController]
[Route("api/habits")]
public class HabitsController : ControllerBase
{
    public IHabitService Habits { get; set; } = null!;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await Habits.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] HabitInput input)
        => Ok(await Habits.CreateAsync(input));

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] HabitInput input)
    {
        try { return Ok(await Habits.UpdateAsync(id, input)); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpPut("{id:long}/toggle")]
    public async Task<IActionResult> Toggle(long id)
    {
        var habit = await Habits.ToggleAsync(id);
        return habit == null ? NotFound() : Ok(habit);
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await Habits.DeleteAsync(id);
        return NoContent();
    }
}
