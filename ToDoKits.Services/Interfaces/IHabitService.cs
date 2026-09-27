using ToDoKits.Models.Dtos;
using ToDoKits.Models.Entities;

namespace ToDoKits.Services.Interfaces;

public interface IHabitService
{
    Task<List<Habit>> GetAllAsync();
    Task<Habit> CreateAsync(HabitInput input);
    Task<Habit> UpdateAsync(long id, HabitInput input);
    Task DeleteAsync(long id);
    Task<Habit?> ToggleAsync(long id);
}
