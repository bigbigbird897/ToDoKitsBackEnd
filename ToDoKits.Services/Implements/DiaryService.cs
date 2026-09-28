using SqlSugar;
using ToDoKits.Command;
using ToDoKits.Models.Dtos;
using ToDoKits.Models.Entities;
using ToDoKits.Services.Interfaces;

namespace ToDoKits.Services.Implements;

public class DiaryService : ServiceBase, IDiaryService
{
    public async Task<List<Diary>> GetAllAsync() =>
        await Db.Queryable<Diary>().Where(d => d.UserId == User.UserId).OrderBy(d => d.Date, OrderByType.Desc).ToListAsync();

    public async Task<Diary> CreateAsync(DiaryInput input)
    {
        var diary = new Diary
        {
            Date = string.IsNullOrWhiteSpace(input.Date) ? DateTime.Now.ToString("yyyy-MM-dd") : input.Date,
            Weekday = input.Weekday,
            Location = input.Location,
            Weather = input.Weather,
            Text = input.Text,
            UserId = User.UserId
        };
        var id = await Db.Insertable(diary).ExecuteReturnIdentityAsync();
        diary.Id = id;
        return diary;
    }

    public async Task<Diary> UpdateAsync(long id, DiaryInput input)
    {
        var diary = await Db.Queryable<Diary>().FirstAsync(d => d.Id == id && d.UserId == User.UserId)
                    ?? throw new KeyNotFoundException($"日记 {id} 不存在");
        if (!string.IsNullOrWhiteSpace(input.Date)) diary.Date = input.Date;
        diary.Weekday = input.Weekday;
        diary.Location = input.Location;
        diary.Weather = input.Weather;
        diary.Text = input.Text;
        await Db.Updateable(diary).ExecuteCommandAsync();
        return diary;
    }

    public async Task DeleteAsync(long id) =>
        await Db.Deleteable<Diary>().Where(d => d.Id == id && d.UserId == User.UserId).ExecuteCommandAsync();
}
