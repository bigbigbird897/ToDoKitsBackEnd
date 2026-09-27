using ToDoKits.Command;
using ToDoKits.Models.Dtos;
using ToDoKits.Models.Entities;
using ToDoKits.Services.Interfaces;

namespace ToDoKits.Services.Implements;

public class StatsService : AppService, IStatsService
{
    private static DateTime Parse(string s)
    {
        var p = s.Split('-');
        return new DateTime(int.Parse(p[0]), int.Parse(p[1]), int.Parse(p[2]));
    }

    private static string Fmt(DateTime d) => $"{d:yyyy-MM-dd}";

    public async Task<RangeStatsResult> RangeAsync(string start, string end)
    {
        var s = Parse(start);
        var e = Parse(end);
        var todos = await Db.Queryable<Todo>().ToListAsync();

        // 范围内完成的事项
        var completed = todos
            .Where(t => t.Status == "done" && t.CompletedAt != null
                        && string.CompareOrdinal(t.CompletedAt, start) >= 0
                        && string.CompareOrdinal(t.CompletedAt, end) <= 0)
            .ToList();

        var cats = completed.GroupBy(t => t.Cat)
            .Select(g => new CategoryStat
            {
                Category = g.Key,
                Count = g.Count(),
                Pct = completed.Count == 0 ? 0 : Math.Round(g.Count() * 100.0 / completed.Count, 1),
                Sample = string.Join("、", g.Take(3).Select(t => t.Name))
            })
            .OrderByDescending(c => c.Count)
            .ToList();

        // 趋势桶：按范围长度自适应粒度
        var days = (int)(e - s).TotalDays + 1;
        var buckets = new List<(string Label, DateTime From, DateTime To)>();
        if (days <= 31)
        {
            for (var i = 0; i < days; i++)
            {
                var d = s.AddDays(i);
                buckets.Add((d.ToString("MM-dd"), d, d));
            }
        }
        else if (days <= 370)
        {
            var cur = new DateTime(s.Year, s.Month, 1);
            while (cur <= e)
            {
                var from = cur;
                var to = cur.AddMonths(1).AddDays(-1);
                buckets.Add(($"{cur:yyyy-MM}", from, to));
                cur = cur.AddMonths(1);
            }
        }
        else
        {
            for (var y = s.Year; y <= e.Year; y++)
            {
                buckets.Add((y.ToString(), new DateTime(y, 1, 1), new DateTime(y, 12, 31)));
            }
        }

        var trend = buckets.Select(b =>
        {
            var inBucket = completed.Where(t => string.CompareOrdinal(t.CompletedAt!, b.From.ToString("yyyy-MM-dd")) >= 0
                                              && string.CompareOrdinal(t.CompletedAt!, b.To.ToString("yyyy-MM-dd")) <= 0).ToList();
            var dict = new Dictionary<string, int>();
            foreach (var c in inBucket.GroupBy(t => t.Cat))
                dict[c.Key] = c.Count();
            return new TrendBucket { Label = b.Label, From = b.From.ToString("yyyy-MM-dd"), To = b.To.ToString("yyyy-MM-dd"), Cats = dict };
        }).ToList();

        return new RangeStatsResult
        {
            Start = start,
            End = end,
            Total = completed.Count,
            ByCategory = cats,
            Trend = trend
        };
    }
}
