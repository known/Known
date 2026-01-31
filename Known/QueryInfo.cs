namespace Known;

/// <summary>
/// 查询信息类。
/// </summary>
public class QueryInfo
{
    /// <summary>
    /// 构造函数，创建一个查询信息类的实例。
    /// </summary>
    public QueryInfo() { }

    /// <summary>
    /// 构造函数，创建一个查询信息类的实例，查询操作类型默认为包含于。
    /// </summary>
    /// <param name="id">查询字段ID。</param>
    /// <param name="value">查询字段值。</param>
    public QueryInfo(string id, string value) : this(id, QueryType.Contain, value) { }

    /// <summary>
    /// 构造函数，创建一个查询信息类的实例。
    /// </summary>
    /// <param name="id">查询字段ID。</param>
    /// <param name="type">查询操作类型。</param>
    /// <param name="value">查询字段值。</param>
    public QueryInfo(string id, QueryType type, string value)
    {
        Id = id;
        Type = type;
        Value = value;
    }

    internal QueryInfo(ColumnInfo column)
    {
        Id = column.Id;
        Type = QueryType.Contain;
        Value = "";
        if (!column.IsQueryAll)
        {
            var codes = Cache.GetCodes(column.Category);
            if (codes != null && codes.Count > 0)
                Value = codes[0].Code;
        }
    }

    /// <summary>
    /// 取得或设置查询字段ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置查询字段值。
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// 取得或设置查询操作类型。
    /// </summary>
    public QueryType Type { get; set; }

    internal bool IsNew { get; set; }
    internal bool IsField { get; set; } = true;
    internal object ParamValue { get; set; }
    internal string[] Values => Value?.Split(',');
}