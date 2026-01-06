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

    internal CommandInfo(DbProvider provider, string text, object param = null) : this(provider, null, text, param)
    {
    }

    internal CommandInfo(DbProvider provider, Type type, string text, object param = null) : this(provider, type, null, text, param)
    {
    }

    internal CommandInfo(DbProvider provider, Type type, string tableName, string text, object param = null)
    {
        Type = type;
        TableName = tableName ?? provider.GetTableName(type);
        Prefix = provider?.Prefix;
        Text = text?.Replace("@", Prefix);
        if (param != null)
        {
            SetParameters(param);
        }
    }

    internal bool IsSave { get; set; }
    internal bool IsClose { get; set; }
    internal string TableName { get; set; }
    internal Type Type { get; set; }
    internal CommandType CmdType { get; set; } = CommandType.Text;
    internal Dictionary<string, object> Original { get; set; }
    internal List<Dictionary<string, object>> DeleteItems { get; set; }

    /// <summary>
    /// 取得SQL参数名称前缀。
    /// </summary>
    public string Prefix { get; }

    /// <summary>
    /// 取得或设置SQL语句。
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// 取得或设置该SQL的Count语句。
    /// </summary>
    public string CountSql { get; set; }

    /// <summary>
    /// 取得或设置该SQL的分页语句。
    /// </summary>
    public string PageSql { get; set; }

    /// <summary>
    /// 取得或设置该SQL的统计语句。
    /// </summary>
    public string StatSql { get; set; }

    /// <summary>
    /// 取得或设置该SQL的Id语句。
    /// </summary>
    public string IdSql { get; set; }

    /// <summary>
    /// 取得或设置该SQL关联的参数列表。
    /// </summary>
    public List<DbParamInfo> Parameters { get; set; } = [];

    internal void SetParameters(DataRow row)
    {
        Parameters = [];
        var keys = new List<string>();
        foreach (DataColumn item in row.Table.Columns)
        {
            keys.Add(item.ColumnName);
            Parameters.Add(item.ColumnName, row[item]);
        }
    }

    internal void SetParameters<T>(T data)
    {
        var parameters = DbUtils.ToDictionary(data);
        foreach (var item in parameters)
        {
            Parameters.Add(item.Key, item.Value);
        }
    }

    /// <summary>
    /// 获取数据库访问命令对象的显示字符串，显示Text和Params内容。
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"Text: {Text}");
        if (Parameters != null && Parameters.Count > 0)
        {
            sb.Append("; Params: ");
            foreach (var item in Parameters)
            {
                sb.Append($"{item.Name}={item.Value},");
            }
        }
        return sb.ToString().TrimEnd(',');
    }
}