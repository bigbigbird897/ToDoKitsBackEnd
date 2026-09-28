using ToDoKits.Command;
using ToDoKits.Models.Entities;
using ToDoKits.Services.Interfaces;

namespace ToDoKits.Services.Implements;

public class CategoryService : ServiceBase, ICategoryService
{
    public async Task<List<string>> GetTodoCatsAsync() =>
        await Db.Queryable<TodoCategory>().Where(c => c.UserId == User.UserId).OrderBy(c => c.Id).Select(c => c.Name).ToListAsync();

    public async Task<List<string>> GetHabitCatsAsync() =>
        await Db.Queryable<HabitCategory>().Where(c => c.UserId == User.UserId).OrderBy(c => c.Id).Select(c => c.Name).ToListAsync();

    public async Task AddTodoCatAsync(string name)
    {
        var n = name.Trim();
        if (string.IsNullOrEmpty(n) || await Db.Queryable<TodoCategory>().AnyAsync(c => c.Name == n && c.UserId == User.UserId)) return;
        await Db.Insertable(new TodoCategory { Name = n, UserId = User.UserId }).ExecuteCommandAsync();
    }

    public async Task DeleteTodoCatAsync(string name)
    {
        await Db.Deleteable<TodoCategory>().Where(c => c.Name == name && c.UserId == User.UserId).ExecuteCommandAsync();
        // 关联待办回退到「工作」
        await Db.Updateable<Todo>().SetColumns(t => t.Cat == "工作").Where(t => t.Cat == name && t.UserId == User.UserId).ExecuteCommandAsync();
    }

    public async Task AddHabitCatAsync(string name)
    {
        var n = name.Trim();
        if (string.IsNullOrEmpty(n) || await Db.Queryable<HabitCategory>().AnyAsync(c => c.Name == n && c.UserId == User.UserId)) return;
        await Db.Insertable(new HabitCategory { Name = n, UserId = User.UserId }).ExecuteCommandAsync();
    }

    public async Task DeleteHabitCatAsync(string name)
    {
        await Db.Deleteable<HabitCategory>().Where(c => c.Name == name && c.UserId == User.UserId).ExecuteCommandAsync();
        await Db.Updateable<Habit>().SetColumns(h => h.Cat == "健康").Where(h => h.Cat == name && h.UserId == User.UserId).ExecuteCommandAsync();
    }
}
