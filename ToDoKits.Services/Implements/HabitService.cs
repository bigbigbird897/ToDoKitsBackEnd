using ToDoKits.Command;
using ToDoKits.Models.Dtos;
using ToDoKits.Models.Entities;
using ToDoKits.Services.Interfaces;

namespace ToDoKits.Services.Implements;

public class HabitService : ServiceBase, IHabitService
{
    public async Task<List<Habit>> GetAllAsync() =>
        await Db.Queryable<Habit>().Where(h => h.UserId == User.UserId).OrderBy(h => h.Id).ToListAsync();

    public async Task<Habit> CreateAsync(HabitInput input)
    {
        var habit = new Habit
        {
            Name = input.Name,
            Cat = string.IsNullOrWhiteSpace(input.Cat) ? "健康" : input.Cat,
            Goal = input.Goal,
            Time = input.Time,
            Streak = 0,
            DoneToday = false,
            UserId = User.UserId
        };
        var id = await Db.Insertable(habit).ExecuteReturnIdentityAsync();
        habit.Id = id;
        return habit;
    }

    public async Task<Habit> UpdateAsync(long id, HabitInput input)
    {
        var habit = await Db.Queryable<Habit>().FirstAsync(h => h.Id == id && h.UserId == User.UserId)
                    ?? throw new KeyNotFoundException($"习惯 {id} 不存在");
        habit.Name = input.Name;
        habit.Cat = string.IsNullOrWhiteSpace(input.Cat) ? "健康" : input.Cat;
        habit.Goal = input.Goal;
        habit.Time = input.Time;
        await Db.Updateable(habit).ExecuteCommandAsync();
        return habit;
    }

    public async Task DeleteAsync(long id) =>
        await Db.Deleteable<Habit>().Where(h => h.Id == id && h.UserId == User.UserId).ExecuteCommandAsync();

    public async Task<Habit?> ToggleAsync(long id)
    {
        var habit = await Db.Queryable<Habit>().FirstAsync(h => h.Id == id && h.UserId == User.UserId);
        if (habit == null) return null;
        if (habit.DoneToday)
        {
            habit.DoneToday = false;
            habit.Streak = Math.Max(0, habit.Streak - 1);
        }
        else
        {
            habit.DoneToday = true;
            habit.Streak += 1;
        }
        await Db.Updateable(habit).ExecuteCommandAsync();
        return habit;
    }
}
