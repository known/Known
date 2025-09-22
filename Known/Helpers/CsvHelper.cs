namespace Known.Helpers;

/// <summary>
/// CSV操作帮助类
/// </summary>
public class CsvHelper
{
    private readonly StringBuilder sb = new();

    /// <summary>
    /// 添加表头。
    /// </summary>
    /// <param name="headers">表头字段数组。</param>
    public void AddHeader(params string[] headers)
    {
        // 添加UTF-8 BOM头
        sb.Append("\uFEFF");
        sb.AppendLine(string.Join(",", headers));
    }

    /// <summary>
    /// 添加数据行。
    /// </summary>
    /// <param name="values">行数据项数组。</param>
    public void AddRow(params string[] values)
    {
        var escapedValues = values.Select(v => v.Contains(",") || v.Contains("\"") ? $"\"{v.Replace("\"", "\"\"")}\"" : v);
        sb.AppendLine(string.Join(",", escapedValues));
    }

    /// <summary>
    /// 转换为文本。
    /// </summary>
    /// <returns></returns>
    public string ToText()
    {
        return sb.ToString();
    }

    /// <summary>
    /// 转换为字节数组。
    /// </summary>
    /// <returns></returns>
    public byte[] ToBytes()
    {
        return Encoding.UTF8.GetBytes(ToText());
    }
}