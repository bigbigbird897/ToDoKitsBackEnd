using SqlSugar;
using ToDoKits.Command;
using ToDoKits.Models.Dtos;
using ToDoKits.Models.Entities;
using ToDoKits.Services.Interfaces;

namespace ToDoKits.Services.Implements;

public class QuoteService : AppService, IQuoteService
{
    public async Task<List<Quote>> GetAllAsync() =>
        await Db.Queryable<Quote>().OrderBy(q => q.Date, OrderByType.Desc).ToListAsync();

    public async Task<Quote> CreateAsync(QuoteInput input)
    {
        var quote = new Quote
        {
            Text = input.Text,
            Who = input.Who,
            Src = input.Src,
            Tags = input.Tags,
            Date = string.IsNullOrWhiteSpace(input.Date) ? DateTime.Now.ToString("yyyy-MM-dd") : input.Date
        };
        var id = await Db.Insertable(quote).ExecuteReturnIdentityAsync();
        quote.Id = id;
        return quote;
    }

    public async Task<Quote> UpdateAsync(long id, QuoteInput input)
    {
        var quote = await Db.Queryable<Quote>().FirstAsync(q => q.Id == id)
                    ?? throw new KeyNotFoundException($"名言 {id} 不存在");
        quote.Text = input.Text;
        quote.Who = input.Who;
        quote.Src = input.Src;
        quote.Tags = input.Tags;
        if (!string.IsNullOrWhiteSpace(input.Date)) quote.Date = input.Date;
        await Db.Updateable(quote).ExecuteCommandAsync();
        return quote;
    }

    public async Task DeleteAsync(long id) =>
        await Db.Deleteable<Quote>().Where(q => q.Id == id).ExecuteCommandAsync();
}
