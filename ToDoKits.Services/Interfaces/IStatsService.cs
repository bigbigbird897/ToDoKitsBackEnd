using ToDoKits.Models.Dtos;

namespace ToDoKits.Services.Interfaces;

/// <summary>统计服务：按指定时间范围统计各类别的完成占比与趋势。</summary>
public interface IStatsService
{
    Task<RangeStatsResult> RangeAsync(string start, string end);
}
