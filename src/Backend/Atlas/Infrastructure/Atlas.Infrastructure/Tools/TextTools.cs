using System.Text;
using Atlas.Application;
using Microsoft.VisualBasic.FileIO;

namespace Atlas.Infrastructure.Tools;

public sealed class TextTools
{
    public string Clean(string input, string operation, string find = "", string replace = "")
    {
        var lines = input.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        return operation switch
        {
            "trim" => string.Join('\n', lines.Select(x => x.Trim())),
            "empty" => string.Join('\n', lines.Where(x => !string.IsNullOrWhiteSpace(x))),
            "unique" => string.Join('\n', lines.Distinct(StringComparer.Ordinal)),
            "sort" => string.Join('\n', lines.Order(StringComparer.Ordinal)),
            "number" => string.Join('\n', lines.Select((x, i) => $"{i + 1}. {x}")),
            "upper" => input.ToUpperInvariant(), "lower" => input.ToLowerInvariant(),
            "replace" when find.Length > 0 => input.Replace(find, replace, StringComparison.Ordinal),
            _ => throw new ArgumentException("Choose a supported text step. Replace needs text to find.")
        };
    }

    public string Workflow(ToolRequest r)
    {
        var result = r.Input;
        var steps = r.Option("steps", "trim,empty,unique").Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (steps.Length is 0 or > 20) throw new ArgumentException("Use between 1 and 20 comma-separated steps.");
        foreach (var step in steps) result = Clean(result, step, r.Option("find"), r.Option("replace"));
        return result;
    }

    public string Diff(string left, string right)
    {
        var a = left.Replace("\r\n", "\n").Split('\n');
        var b = right.Replace("\r\n", "\n").Split('\n');
        // ponytail: bounded line LCS; use a streaming diff if large log comparison is needed.
        if (a.Length > 2000 || b.Length > 2000) throw new ArgumentException("Compare up to 2,000 lines per side.");
        var lengths = new int[a.Length + 1, b.Length + 1];
        for (var i = a.Length - 1; i >= 0; i--)
        for (var j = b.Length - 1; j >= 0; j--)
            lengths[i, j] = a[i] == b[j] ? 1 + lengths[i + 1, j + 1] : Math.Max(lengths[i + 1, j], lengths[i, j + 1]);
        var output = new StringBuilder();
        int x = 0, y = 0;
        while (x < a.Length || y < b.Length)
        {
            if (x < a.Length && y < b.Length && a[x] == b[y]) { output.AppendLine("  " + a[x++]); y++; }
            else if (x < a.Length && (y == b.Length || lengths[x + 1, y] >= lengths[x, y + 1])) output.AppendLine("− " + a[x++]);
            else output.AppendLine("+ " + b[y++]);
        }
        return output.ToString();
    }

    public ToolResult Csv(ToolRequest r)
    {
        using var parser = new TextFieldParser(new StringReader(r.Input)) { TextFieldType = FieldType.Delimited, HasFieldsEnclosedInQuotes = true, TrimWhiteSpace = false };
        var delimiter = r.Option("delimiter", ",");
        if (delimiter == "tab") delimiter = "\t";
        if (delimiter.Length != 1) throw new ArgumentException("Choose a one-character delimiter or tab.");
        parser.SetDelimiters(delimiter);
        var rows = new List<string[]>();
        while (!parser.EndOfData)
        {
            if (rows.Count >= 10000) throw new ArgumentException("CSV preview supports up to 10,000 rows.");
            rows.Add(parser.ReadFields() ?? []);
        }
        if (rows.Count == 0) return new("", Rows: []);
        var width = rows.Max(x => x.Length);
        if (width > 100) throw new ArgumentException("CSV preview supports up to 100 columns.");
        var columns = string.IsNullOrWhiteSpace(r.Option("columns")) ? Enumerable.Range(0, width).ToArray() :
            r.Option("columns").Split(',').Select(x => int.TryParse(x.Trim(), out var n) && n >= 1 && n <= width ? n - 1 : throw new ArgumentException("Column numbers start at 1.")).ToArray();
        rows = rows.Select(row => columns.Select(i => row.ElementAtOrDefault(i) ?? "").ToArray()).ToList();
        if (r.Flag("unique")) rows = rows.DistinctBy(row => System.Text.Json.JsonSerializer.Serialize(row)).ToList();
        static string Quote(string value) => "\"" + value.Replace("\"", "\"\"") + "\"";
        var csv = string.Join('\n', rows.Select(row => string.Join(delimiter, row.Select(Quote))));
        return new(csv, Rows: rows.ToArray());
    }
}
