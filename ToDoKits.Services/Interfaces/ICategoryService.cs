namespace ToDoKits.Services.Interfaces;

/// <summary>分类服务：待办类别与习惯类别分别维护，均可新增 / 删除。</summary>
public interface ICategoryService
{
    Task<List<string>> GetTodoCatsAsync();
    Task<List<string>> GetHabitCatsAsync();
    Task AddTodoCatAsync(string name);
    Task DeleteTodoCatAsync(string name);
    Task AddHabitCatAsync(string name);
    Task DeleteHabitCatAsync(string name);
}
