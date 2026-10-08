using ToDoKits.Models.Dtos;
using ToDoKits.Models.Entities;

namespace ToDoKits.Services.Interfaces;

/// <summary>工作笔记服务：文件夹树 + 文件（txt / md）增删改查与内容读写。</summary>
public interface IWorkNotesService
{
    /// <summary>当前账号全部文件夹（扁平列表，前端按 ParentId 组装树）。</summary>
    Task<List<WorkFolder>> GetFoldersAsync();

    /// <summary>新建文件夹（parentId 为 null 表示根目录）。</summary>
    Task<WorkFolder> AddFolderAsync(string name, long? parentId);

    /// <summary>重命名文件夹。</summary>
    Task<WorkFolder> RenameFolderAsync(long id, string name);

    /// <summary>删除文件夹（连同其子文件夹与文件一起删除）。</summary>
    Task DeleteFolderAsync(long id);

    /// <summary>某文件夹下的文件列表（不含大段 Content，供列表展示）。</summary>
    Task<List<WorkFile>> GetFilesAsync(long folderId);

    /// <summary>当前账号全部文件（含 Content，供导出）。</summary>
    Task<List<WorkFile>> GetAllFilesAsync();

    /// <summary>取单个文件完整信息（含 Content，用于查看 / 编辑 / 下载）。</summary>
    Task<WorkFile> GetFileAsync(long id);

    /// <summary>新建文件。</summary>
    Task<WorkFile> CreateFileAsync(WorkFileInput input);

    /// <summary>更新文件（改名 / 改类型 / 改内容）。</summary>
    Task<WorkFile> UpdateFileAsync(long id, WorkFileInput input);

    /// <summary>删除文件。</summary>
    Task DeleteFileAsync(long id);
}
