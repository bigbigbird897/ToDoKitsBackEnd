using ToDoKits.Models.Dtos;
using ToDoKits.Models.Entities;

namespace ToDoKits.Services.Interfaces;

public interface IQuoteService
{
    Task<List<Quote>> GetAllAsync();
    Task<Quote> CreateAsync(QuoteInput input);
    Task<Quote> UpdateAsync(long id, QuoteInput input);
    Task DeleteAsync(long id);
}
