namespace Known.Data;

public class CommandInfo
{
    public CommandInfo() { }

    internal CommandInfo(DbProvider builder, string text, object param = null)
    {
        Prefix = builder.Prefix;
        Text = text?.Replace("@", Prefix);
        if (param != null)
            Params = DBUtils.ToDictionary(param);
        Config.App.DBLog?.Invoke(this);
    }

    internal bool IsSave { get; set; }
    internal bool IsClose { get; set; }
    public string Prefix { get; }
    public string Text { get; internal set; }
    public string CountSql { get; internal set; }
    public string PageSql { get; internal set; }
    public string StatSql { get; internal set; }
    public Dictionary<string, object> Params { get; internal set; }

    internal void SetParameters(DataRow row)
    {
        Params = [];
        var keys = new List<string>();
        foreach (DataColumn item in row.Table.Columns)
        {
            keys.Add(item.ColumnName);
            Params.Add(item.ColumnName, row[item]);
        }
    }

    internal void SetParameters<T>(T data) => Params = DBUtils.ToDictionary(data);

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Text:{Text}");
        if (Params != null && Params.Count > 0)
        {
            sb.AppendLine(", Params:");
            foreach (var item in Params)
            {
                sb.AppendLine($"{item.Key}={item.Value}");
            }
        }
        return sb.ToString();
    }
}