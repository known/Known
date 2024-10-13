namespace Known.Data;

/// <summary>
/// 数据库访问命令信息类。
/// </summary>
public class CommandInfo
{
    /// <summary>
    /// 构造函数，创建一个数据库访问命令信息类的实例。
    /// </summary>
    public CommandInfo() { }

    internal CommandInfo(DbProvider builder, string text, object param = null)
    {
        Prefix = builder.Prefix;
        Text = text?.Replace("@", Prefix);
        if (param != null)
            Params = DbUtils.ToDictionary(param);
    }

    internal bool IsSave { get; set; }
    internal bool IsClose { get; set; }

    /// <summary>
    /// 取得SQL参数名称前缀。
    /// </summary>
    public string Prefix { get; }

    /// <summary>
    /// 取得SQL语句。
    /// </summary>
    public string Text { get; internal set; }

    /// <summary>
    /// 取得该SQL的Count语句。
    /// </summary>
    public string CountSql { get; internal set; }

    /// <summary>
    /// 取得该SQL的分页语句。
    /// </summary>
    public string PageSql { get; internal set; }

    /// <summary>
    /// 取得该SQL的统计语句。
    /// </summary>
    public string StatSql { get; internal set; }

    /// <summary>
    /// 取得该SQL关联的参数字典。
    /// </summary>
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

    internal void SetParameters<T>(T data) => Params = DbUtils.ToDictionary(data);

    /// <summary>
    /// 获取数据库访问命令对象的显示字符串，显示Text和Params内容。
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"Text: {Text}");
        if (Params != null && Params.Count > 0)
        {
            sb.Append("; Params: ");
            foreach (var item in Params)
            {
                sb.Append($"{item.Key}={item.Value},");
            }
        }
        return sb.ToString().TrimEnd(',');
    }
}