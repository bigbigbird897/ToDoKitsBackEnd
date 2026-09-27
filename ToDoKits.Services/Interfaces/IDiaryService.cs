using ToDoKits.Models.Dtos;
using ToDoKits.Models.Entities;

namespace ToDoKits.Services.Interfaces;

public interface IDiaryService
{
    Task<List<Diary>> GetAllAsync();
    Task<Diary> CreateAsync(DiaryInput input);
    Task<Diary> UpdateAsync(long id, DiaryInput input);
    Task DeleteAsync(long id);
}
