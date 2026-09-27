using SqlSugar;
using ToDoKits.Command;
using ToDoKits.Models.Dtos;
using ToDoKits.Models.Entities;
using ToDoKits.Services.Interfaces;

namespace ToDoKits.Services.Implements;

public class ReadingService : AppService, IReadingService
{
    public async Task<List<string>> GetFoldersAsync() =>
        await Db.Queryable<Folder>().OrderBy(f => f.Id).Select(f => f.Name).ToListAsync();

    public async Task AddFolderAsync(string name)
    {
        var n = name.Trim();
        if (string.IsNullOrEmpty(n) || await Db.Queryable<Folder>().AnyAsync(f => f.Name == n)) return;
        await Db.Insertable(new Folder { Name = n }).ExecuteCommandAsync();
    }

    public async Task DeleteFolderAsync(string name)
    {
        await Db.Deleteable<Folder>().Where(f => f.Name == name).ExecuteCommandAsync();
        await Db.Deleteable<Note>().Where(n => n.Folder == name).ExecuteCommandAsync();
    }

    public async Task<List<Note>> GetNotesAsync() =>
        await Db.Queryable<Note>().OrderBy(n => n.Date, OrderByType.Desc).ToListAsync();

    public async Task<Note> CreateNoteAsync(NoteInput input)
    {
        var note = new Note
        {
            Folder = input.Folder,
            Title = input.Title,
            Content = input.Content,
            Date = string.IsNullOrWhiteSpace(input.Date) ? DateTime.Now.ToString("yyyy-MM-dd") : input.Date
        };
        var id = await Db.Insertable(note).ExecuteReturnIdentityAsync();
        note.Id = id;
        return note;
    }

    public async Task<Note> UpdateNoteAsync(long id, NoteInput input)
    {
        var note = await Db.Queryable<Note>().FirstAsync(n => n.Id == id)
                   ?? throw new KeyNotFoundException($"笔记 {id} 不存在");
        note.Folder = input.Folder;
        note.Title = input.Title;
        note.Content = input.Content;
        if (!string.IsNullOrWhiteSpace(input.Date)) note.Date = input.Date;
        await Db.Updateable(note).ExecuteCommandAsync();
        return note;
    }

    public async Task DeleteNoteAsync(long id) =>
        await Db.Deleteable<Note>().Where(n => n.Id == id).ExecuteCommandAsync();
}
