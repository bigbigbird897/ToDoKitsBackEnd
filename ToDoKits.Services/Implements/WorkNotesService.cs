using SqlSugar;
using ToDoKits.Models.Dtos;
using ToDoKits.Models.Entities;
using ToDoKits.Services.Interfaces;

namespace ToDoKits.Services.Implements;

/// <summary>工作笔记服务实现：文件夹树 + 文件增删改查，按登录账号隔离。</summary>
public class WorkNotesService : ServiceBase, IWorkNotesService
{
    public async Task<List<WorkFolder>> GetFoldersAsync() =>
        await Db.Queryable<WorkFolder>().Where(f => f.UserId == User.UserId).OrderBy(f => f.Id).ToListAsync();

    public async Task<WorkFolder> AddFolderAsync(string name, long? parentId)
    {
        var n = name.Trim();
        if (string.IsNullOrEmpty(n)) throw new ArgumentException("文件夹名称不能为空");
        // 同一父目录下不允许重名
        var dup = await Db.Queryable<WorkFolder>().AnyAsync(f => f.Name == n && f.ParentId == parentId && f.UserId == User.UserId);
        if (dup) throw new ArgumentException("同级已存在同名文件夹");
        var folder = new WorkFolder
        {
            Name = n,
            ParentId = parentId,
            UserId = User.UserId,
            CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
        };
        folder.Id = await Db.Insertable(folder).ExecuteReturnIdentityAsync();
        return folder;
    }

    public async Task<WorkFolder> RenameFolderAsync(long id, string name)
    {
        var folder = await Db.Queryable<WorkFolder>().FirstAsync(f => f.Id == id && f.UserId == User.UserId)
                     ?? throw new KeyNotFoundException($"文件夹 {id} 不存在");
        var n = name.Trim();
        if (string.IsNullOrEmpty(n)) throw new ArgumentException("文件夹名称不能为空");
        var dup = await Db.Queryable<WorkFolder>().AnyAsync(f => f.Name == n && f.ParentId == folder.ParentId && f.Id != id && f.UserId == User.UserId);
        if (dup) throw new ArgumentException("同级已存在同名文件夹");
        folder.Name = n;
        await Db.Updateable(folder).ExecuteCommandAsync();
        return folder;
    }

    public async Task DeleteFolderAsync(long id)
    {
        // 级联：收集该文件夹及其所有后代文件夹 Id，一并删除其下文件
        var all = await Db.Queryable<WorkFolder>().Where(f => f.UserId == User.UserId).ToListAsync();
        var toDel = new List<long>();
        void Collect(long pid)
        {
            foreach (var f in all.Where(x => x.ParentId == pid))
            {
                toDel.Add(f.Id);
                Collect(f.Id);
            }
        }
        toDel.Add(id);
        Collect(id);

        await Db.Deleteable<WorkFolder>().Where(f => toDel.Contains(f.Id) && f.UserId == User.UserId).ExecuteCommandAsync();
        await Db.Deleteable<WorkFile>().Where(f => toDel.Contains(f.FolderId) && f.UserId == User.UserId).ExecuteCommandAsync();
    }

    // 列表不返回大段 Content，仅返回元信息（查看 / 编辑 / 下载时再取完整）
    public async Task<List<WorkFile>> GetFilesAsync(long folderId) =>
        await Db.Queryable<WorkFile>()
            .Where(f => f.FolderId == folderId && f.UserId == User.UserId)
            .OrderBy(f => f.Name)
            .Select(f => new WorkFile { Id = f.Id, UserId = f.UserId, FolderId = f.FolderId, Name = f.Name, Type = f.Type, UpdatedAt = f.UpdatedAt })
            .ToListAsync();

    public async Task<WorkFile> GetFileAsync(long id) =>
        await Db.Queryable<WorkFile>().FirstAsync(f => f.Id == id && f.UserId == User.UserId)
        ?? throw new KeyNotFoundException($"文件 {id} 不存在");

    public async Task<List<WorkFile>> GetAllFilesAsync() =>
        await Db.Queryable<WorkFile>().Where(f => f.UserId == User.UserId).OrderBy(f => f.FolderId).ToListAsync();

    public async Task<WorkFile> CreateFileAsync(WorkFileInput input)
    {
        var n = input.Name.Trim();
        if (string.IsNullOrEmpty(n)) throw new ArgumentException("文件名不能为空");
        var type = ResolveType(n, input.Type);
        var file = new WorkFile
        {
            Name = n,
            FolderId = input.FolderId,
            Type = type,
            Content = input.Content ?? "",
            UserId = User.UserId,
            UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
        };
        file.Id = await Db.Insertable(file).ExecuteReturnIdentityAsync();
        return file;
    }

    public async Task<WorkFile> UpdateFileAsync(long id, WorkFileInput input)
    {
        var file = await Db.Queryable<WorkFile>().FirstAsync(f => f.Id == id && f.UserId == User.UserId)
                   ?? throw new KeyNotFoundException($"文件 {id} 不存在");
        if (!string.IsNullOrWhiteSpace(input.Name))
        {
            file.Name = input.Name.Trim();
            file.Type = ResolveType(file.Name, input.Type);
        }
        else if (!string.IsNullOrWhiteSpace(input.Type))
        {
            file.Type = input.Type;
        }
        if (input.Content != null) file.Content = input.Content;
        file.UpdatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        await Db.Updateable(file).ExecuteCommandAsync();
        return file;
    }

    public async Task DeleteFileAsync(long id) =>
        await Db.Deleteable<WorkFile>().Where(f => f.Id == id && f.UserId == User.UserId).ExecuteCommandAsync();

    /// <summary>按文件名后缀解析类型；未带后缀时用请求里的 type。</summary>
    private static string ResolveType(string name, string fallback)
    {
        if (name.EndsWith(".md", StringComparison.OrdinalIgnoreCase)) return "md";
        if (name.EndsWith(".txt", StringComparison.OrdinalIgnoreCase)) return "txt";
        return string.Equals(fallback, "md", StringComparison.OrdinalIgnoreCase) ? "md" : "txt";
    }
}
