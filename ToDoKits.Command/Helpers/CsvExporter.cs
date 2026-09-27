using System.Text;

namespace ToDoKits.Command.Helpers;

/// <summary>
/// 通用 CSV 导出辅助：把任意行的对象投影成 (列名, 值) 生成 CSV，并输出带 BOM 的 UTF-8，
/// 便于 Excel 直接打开中文。
/// </summary>
public static class CsvExporter
{
    public static string ToCsv<T>(IEnumerable<T> rows, Func<T, (string Name, string Value)[]> project)
    {
        var list = rows.ToList();
        var sb = new StringBuilder();
        var headers = new List<string>();
        foreach (var (name, _) in list.SelectMany(project).GroupBy(x => x.Name).Select(g => g.First()))
        {
            headers.Add(name);
        }
        if (headers.Count > 0)
        {
            sb.AppendLine(string.Join(',', headers.Select(Escape)));
            foreach (var r in list)
            {
                var cols = project(r).ToDictionary(x => x.Name, x => x.Value);
                sb.AppendLine(string.Join(',', headers.Select(h => Escape(cols.GetValueOrDefault(h, "")))));
            }
        }
        return sb.ToString();
    }

    private static string Escape(string s) => "\"" + (s ?? "").Replace("\"", "\"\"") + "\"";

    /// <summary>带 UTF-8 BOM 的字节，供 File 下载使用。</summary>
    public static byte[] ToUtf8WithBom(string csv)
    {
        var preamble = Encoding.UTF8.GetPreamble();
        var body = Encoding.UTF8.GetBytes(csv);
        var result = new byte[preamble.Length + body.Length];
        Buffer.BlockCopy(preamble, 0, result, 0, preamble.Length);
        Buffer.BlockCopy(body, 0, result, preamble.Length, body.Length);
        return result;
    }
}
