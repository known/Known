namespace Known.Data;

public class CommandInfo
{
    public CommandInfo() { }

    internal CommandInfo(SqlBuilder builder, string text, object param = null)
    {
        Prefix = builder.Prefix;
        Text = text?.Replace("@", Prefix);
        if (param != null)
            Params = MapToDictionary(param);
    }

    internal bool IsSave { get; set; }
    internal bool IsClose { get; set; }
    public string Prefix { get; }
    public string Text { get; set; }
    public string CountSql { get; set; }
    public string PageSql { get; set; }
    public string SumSql { get; set; }
    public Dictionary<string, object> Params { get; set; }

    public void SetParameters(DataRow row)
    {
        Params = [];
        var keys = new List<string>();
        foreach (DataColumn item in row.Table.Columns)
        {
            keys.Add(item.ColumnName);
            Params.Add(item.ColumnName, row[item]);
        }
    }

    public void SetParameters<T>(T data)
    {
        Params = MapToDictionary(data);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine(Text);
        if (Params != null && Params.Count > 0)
        {
            foreach (var item in Params)
            {
                sb.AppendLine($"{item.Key}={item.Value}");
            }
        }
        return sb.ToString();
    }

    internal static Dictionary<string, object> MapToDictionary(object value)
    {
        if (value is Dictionary<string, object> dictionary)
            return dictionary;

        var dic = Utils.MapTo<Dictionary<string, object>>(value);
        return dic ?? [];
    }
}