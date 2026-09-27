using ToDoKits.Models.Dtos;
using ToDoKits.Models.Entities;

namespace ToDoKits.Services.Interfaces;

/// <summary>读后感：文件夹 CRUD + 笔记 CRUD。</summary>
public interface IReadingService
{
    Task<List<string>> GetFoldersAsync();
    Task AddFolderAsync(string name);
    Task DeleteFolderAsync(string name);
    Task<List<Note>> GetNotesAsync();
    Task<Note> CreateNoteAsync(NoteInput input);
    Task<Note> UpdateNoteAsync(long id, NoteInput input);
    Task DeleteNoteAsync(long id);
}
