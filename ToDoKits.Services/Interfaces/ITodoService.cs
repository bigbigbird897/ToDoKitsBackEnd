using ToDoKits.Models.Dtos;
using ToDoKits.Models.Entities;

namespace ToDoKits.Services.Interfaces;

public interface ITodoService
{
    Task<List<Todo>> GetAllAsync();
    Task<Todo?> GetAsync(long id);
    Task<Todo> CreateAsync(TodoInput input);
    Task<Todo> UpdateAsync(long id, TodoInput input);
    Task DeleteAsync(long id);
    Task<Todo?> ToggleAsync(long id);
}
