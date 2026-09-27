using Microsoft.AspNetCore.Mvc;
using ToDoKits.Services.Interfaces;

namespace ToDoKits.Controllers;

/// <summary>数据统计接口：/api/stats/range?start=&amp;end=</summary>
[ApiController]
[Route("api/stats")]
public class StatsController : ControllerBase
{
    public IStatsService Stats { get; set; } = null!;

    [HttpGet("range")]
    public async Task<IActionResult> Range([FromQuery] string start, [FromQuery] string end)
        => Ok(await Stats.RangeAsync(start, end));
}
