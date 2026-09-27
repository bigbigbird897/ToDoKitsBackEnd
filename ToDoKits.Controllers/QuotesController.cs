using Microsoft.AspNetCore.Mvc;
using ToDoKits.Models.Dtos;
using ToDoKits.Services.Interfaces;

namespace ToDoKits.Controllers;

/// <summary>名言警句接口：/api/quotes</summary>
[ApiController]
[Route("api/quotes")]
public class QuotesController : ControllerBase
{
    public IQuoteService Quotes { get; set; } = null!;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await Quotes.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] QuoteInput input)
        => Ok(await Quotes.CreateAsync(input));

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(long id, [FromBody] QuoteInput input)
    {
        try { return Ok(await Quotes.UpdateAsync(id, input)); }
        catch (KeyNotFoundException) { return NotFound(); }
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await Quotes.DeleteAsync(id);
        return NoContent();
    }
}
