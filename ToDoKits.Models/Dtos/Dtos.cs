namespace ToDoKits.Models.Dtos;

/// <summary>待办新增 / 更新的输入。</summary>
public class TodoInput
{
    public string Name { get; set; } = "";
    public string Cat { get; set; } = "工作";
    public string Start { get; set; } = "";
    public string? Due { get; set; }
    public string Status { get; set; } = "doing";
    public string Repeat { get; set; } = "";
    public string? Note { get; set; }
}

/// <summary>习惯新增 / 更新的输入。</summary>
public class HabitInput
{
    public string Name { get; set; } = "";
    public string Cat { get; set; } = "健康";
    public string? Goal { get; set; }
    public string? Time { get; set; }
}

/// <summary>名言新增 / 更新的输入。</summary>
public class QuoteInput
{
    public string Text { get; set; } = "";
    public string? Who { get; set; }
    public string? Src { get; set; }
    public string? Tags { get; set; }
    public string Date { get; set; } = "";
}

/// <summary>读后感笔记新增 / 更新的输入。</summary>
public class NoteInput
{
    public string Folder { get; set; } = "";
    public string Title { get; set; } = "";
    public string? Content { get; set; }
    public string Date { get; set; } = "";
}

/// <summary>日记新增 / 更新的输入。</summary>
public class DiaryInput
{
    public string Date { get; set; } = "";
    public string? Weekday { get; set; }
    public string? Location { get; set; }
    public string? Weather { get; set; }
    public string? Text { get; set; }
}

/// <summary>统计：某一类别在范围内的完成情况。</summary>
public class CategoryStat
{
    public string Category { get; set; } = "";
    public int Count { get; set; }
    public double Pct { get; set; }
    public string Sample { get; set; } = "";
}

/// <summary>统计：趋势图的一个时间桶（按日 / 按月 / 按年）。</summary>
public class TrendBucket
{
    public string Label { get; set; } = "";
    public string From { get; set; } = "";
    public string To { get; set; } = "";
    public Dictionary<string, int> Cats { get; set; } = new();
}

/// <summary>时间范围统计结果（类别占比 + 趋势）。</summary>
public class RangeStatsResult
{
    public string Start { get; set; } = "";
    public string End { get; set; } = "";
    public int Total { get; set; }
    public List<CategoryStat> ByCategory { get; set; } = new();
    public List<TrendBucket> Trend { get; set; } = new();
}
