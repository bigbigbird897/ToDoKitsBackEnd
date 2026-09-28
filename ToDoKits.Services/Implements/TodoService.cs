using SqlSugar;
using ToDoKits.Command;
using ToDoKits.Models.Dtos;
using ToDoKits.Models.Entities;
using ToDoKits.Services.Interfaces;

namespace ToDoKits.Services.Implements;

public class TodoService : ServiceBase, ITodoService
{
    private static string Today() => DateTime.Now.ToString("yyyy-MM-dd");

    public async Task<List<Todo>> GetAllAsync() =>
        await Db.Queryable<Todo>().Where(t => t.UserId == User.UserId).OrderBy(t => t.Start, OrderByType.Desc).ToListAsync();

    public async Task<Todo?> GetAsync(long id) =>
        await Db.Queryable<Todo>().FirstAsync(t => t.Id == id && t.UserId == User.UserId);

    public async Task<Todo> CreateAsync(TodoInput input)
    {
        var todo = new Todo
        {
            Name = input.Name,
            Cat = string.IsNullOrWhiteSpace(input.Cat) ? "工作" : input.Cat,
            Start = string.IsNullOrWhiteSpace(input.Start) ? Today() : input.Start,
            Due = input.Due,
            Status = input.Status == "done" ? "done" : "doing",
            Repeat = input.Repeat ?? "",
            Note = input.Note,
            CompletedAt = input.Status == "done" ? Today() : null,
            UserId = User.UserId
        };
        var id = await Db.Insertable(todo).ExecuteReturnIdentityAsync();
        todo.Id = id;
        return todo;
    }

    public async Task<Todo> UpdateAsync(long id, TodoInput input)
    {
        var todo = await Db.Queryable<Todo>().FirstAsync(t => t.Id == id && t.UserId == User.UserId)
                   ?? throw new KeyNotFoundException($"待办 {id} 不存在");
        todo.Name = input.Name;
        todo.Cat = string.IsNullOrWhiteSpace(input.Cat) ? "工作" : input.Cat;
        todo.Start = string.IsNullOrWhiteSpace(input.Start) ? Today() : input.Start;
        todo.Due = input.Due;
        todo.Repeat = input.Repeat ?? "";
        todo.Note = input.Note;
        var wasDone = todo.Status == "done";
        todo.Status = input.Status == "done" ? "done" : "doing";
        todo.CompletedAt = todo.Status == "done" && !wasDone ? Today() : todo.Status != "done" ? null : todo.CompletedAt;
        await Db.Updateable(todo).ExecuteCommandAsync();
        return todo;
    }

    public async Task DeleteAsync(long id) =>
        await Db.Deleteable<Todo>().Where(t => t.Id == id && t.UserId == User.UserId).ExecuteCommandAsync();

    public async Task<Todo?> ToggleAsync(long id)
    {
        var todo = await Db.Queryable<Todo>().FirstAsync(t => t.Id == id && t.UserId == User.UserId);
        if (todo == null) return null;
        todo.Status = todo.Status == "done" ? "doing" : "done";
        todo.CompletedAt = todo.Status == "done" ? Today() : null;
        await Db.Updateable(todo).ExecuteCommandAsync();
        return todo;
    }
}
