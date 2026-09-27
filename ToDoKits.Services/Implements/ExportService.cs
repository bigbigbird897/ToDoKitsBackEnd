using ToDoKits.Command;
using ToDoKits.Command.Helpers;
using ToDoKits.Models.Entities;
using ToDoKits.Services.Interfaces;

namespace ToDoKits.Services.Implements;

public class ExportService : AppService, IExportService
{
    public (string FileName, byte[] Bytes) ExportModule(string module)
    {
        var table = module.ToLowerInvariant() switch
        {
            "todo" => ("待办事项", BuildTodo()),
            "habit" => ("好习惯", BuildHabit()),
            "quote" => ("名言警句", BuildQuote()),
            "note" => ("读后感", BuildNote()),
            "diary" => ("电子日记", BuildDiary()),
            _ => throw new KeyNotFoundException($"未知导出模块：{module}")
        };
        return ($"{table.Item1}-{DateTime.Now:yyyyMMdd}.csv", CsvExporter.ToUtf8WithBom(table.Item2));
    }

    public Dictionary<string, byte[]> ExportAll() => new()
    {
        ["待办事项.csv"] = CsvExporter.ToUtf8WithBom(BuildTodo()),
        ["好习惯.csv"] = CsvExporter.ToUtf8WithBom(BuildHabit()),
        ["名言警句.csv"] = CsvExporter.ToUtf8WithBom(BuildQuote()),
        ["读后感.csv"] = CsvExporter.ToUtf8WithBom(BuildNote()),
        ["电子日记.csv"] = CsvExporter.ToUtf8WithBom(BuildDiary())
    };

    private string BuildTodo() =>
        CsvExporter.ToCsv(Db.Queryable<Todo>().OrderByDescending(t => t.Start).ToList(),
            t => new[] { ("名称", t.Name), ("类别", t.Cat), ("开始", t.Start), ("预计完成", t.Due ?? ""), ("状态", t.Status == "done" ? "已完成" : "未完成"), ("周期", t.Repeat), ("完成时间", t.CompletedAt ?? ""), ("备注", t.Note ?? "") });

    private string BuildHabit() =>
        CsvExporter.ToCsv(Db.Queryable<Habit>().ToList(),
            h => new[] { ("名称", h.Name), ("类别", h.Cat), ("目标", h.Goal ?? ""), ("连续天数", h.Streak.ToString()), ("提醒时间", h.Time ?? ""), ("今日打卡", h.DoneToday ? "是" : "否") });

    private string BuildQuote() =>
        CsvExporter.ToCsv(Db.Queryable<Quote>().OrderByDescending(q => q.Date).ToList(),
            q => new[] { ("内容", q.Text), ("作者", q.Who ?? ""), ("出处", q.Src ?? ""), ("标签", q.Tags ?? ""), ("日期", q.Date) });

    private string BuildNote() =>
        CsvExporter.ToCsv(Db.Queryable<Note>().OrderByDescending(n => n.Date).ToList(),
            n => new[] { ("文件夹", n.Folder), ("标题", n.Title), ("日期", n.Date), ("内容", n.Content ?? "") });

    private string BuildDiary() =>
        CsvExporter.ToCsv(Db.Queryable<Diary>().OrderByDescending(d => d.Date).ToList(),
            d => new[] { ("日期", d.Date), ("星期", d.Weekday ?? ""), ("地点", d.Location ?? ""), ("天气", d.Weather ?? ""), ("内容", d.Text ?? "") });
}
