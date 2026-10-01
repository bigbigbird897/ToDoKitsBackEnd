using ToDoKits.Command;
using ToDoKits.Models.Dtos;
using ToDoKits.Models.Entities;
using ToDoKits.Services.Interfaces;

namespace ToDoKits.Services.Implements;

public class HabitService : ServiceBase, IHabitService
{
    public async Task<List<Habit>> GetAllAsync()
    {
        var today = DateTime.Now.ToString("yyyy-MM-dd");
        var yesterday = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
        var list = await Db.Queryable<Habit>().Where(h => h.UserId == User.UserId).OrderBy(h => h.Id).ToListAsync();
        // 跨天归一（只影响返回给前端的值，不写库）：
        //  - 今天未打卡 → DoneToday 置 false（解决"第二天勾选框不刷新"）
        //  - 上次打卡既非今天也非昨天 → 连续打卡天数归零（连续已中断）
        foreach (var h in list)
        {
            if (h.LastDoneDate != today)
            {
                h.DoneToday = false;
                if (h.LastDoneDate != yesterday)
                    h.Streak = 0;
            }
        }
        return list;
    }

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
        var today = DateTime.Now.ToString("yyyy-MM-dd");
        var yesterday = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");

        if (habit.LastDoneDate == today)
        {
            // 今天已打过卡 → 再点取消打卡
            habit.DoneToday = false;
            habit.Streak = Math.Max(0, habit.Streak - 1);
            habit.LastDoneDate = null;
        }
        else
        {
            // 今天第一次打卡：昨天也打过 → 连续天数 +1；否则连续从 1 重新计数
            habit.DoneToday = true;
            habit.Streak = (habit.LastDoneDate == yesterday) ? habit.Streak + 1 : 1;
            habit.LastDoneDate = today;
        }
        await Db.Updateable(habit).ExecuteCommandAsync();
        return habit;
    }
}
