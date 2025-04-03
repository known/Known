namespace Known;

/// <summary>
/// 查询条件类。
/// </summary>
public class PagingCriteria
{
    /// <summary>
    /// 构造函数，创建一个查询条件类的实例。
    /// </summary>
    public PagingCriteria()
    {
        PageSize = Config.App.DefaultPageSize;
        Clear();
    }

    /// <summary>
    /// 取得或设置导出模式。
    /// </summary>
    public ExportMode ExportMode { get; set; }

    /// <summary>
    /// 取得或设置导出栏位信息列表。
    /// </summary>
    public List<ExportColumnInfo> ExportColumns { get; set; }

    /// <summary>
    /// 取得或设置统计栏位信息列表。
    /// </summary>
    public List<StatisticColumnInfo> StatisticColumns { get; set; }

    /// <summary>
    /// 取得或设置扩展查询参数。
    /// </summary>
    public Dictionary<string, object> Parameters { get; set; }

    /// <summary>
    /// 取得或设置是否是页面加载查询。
    /// </summary>
    public bool IsLoad { get; set; }

    /// <summary>
    /// 取得或设置是否是点击查询按钮查询。
    /// </summary>
    public bool IsQuery { get; set; }

    /// <summary>
    /// 取得或设置查询页码。
    /// </summary>
    public int PageIndex { get; set; }

    /// <summary>
    /// 取得或设置每页查询大小。
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 取得或设置查询条件信息列表。
    /// </summary>
    public List<QueryInfo> Query { get; set; }

    /// <summary>
    /// 取得或设置查询排序字段集合。
    /// </summary>
    public string[] OrderBys { get; set; }

    /// <summary>
    /// 取得或设置查询字段别名字典，适用多表联合查询字段名重复是设置字段别名。
    /// </summary>
    public Dictionary<string, string> Fields { get; set; }

    internal Dictionary<string, object> CmdParams { get; set; }

    /// <summary>
    /// 清空初始化查询条件对象。
    /// </summary>
    public void Clear()
    {
        IsLoad = true;
        IsQuery = false;
        ExportColumns = [];
        Parameters = [];
        StatisticColumns = [];
        Query = [];
        OrderBys = [];
        Fields = [];
        PageIndex = 1;
    }

    /// <summary>
    /// 获取查询字段值。
    /// </summary>
    /// <param name="id">查询字段ID。</param>
    /// <returns>查询字段值。</returns>
    public string GetQueryValue(string id)
    {
        var query = Query?.FirstOrDefault(q => q.Id == id);
        if (query == null)
            return string.Empty;

        return query.Value;
    }

    /// <summary>
    /// 设置查询条件信息，条件默认包含于。
    /// </summary>
    /// <param name="id">字段属性ID。</param>
    /// <param name="value">查询条件值。</param>
    /// <returns>查询条件信息。</returns>
    public QueryInfo SetQuery(string id, string value)
    {
        return SetQuery(id, QueryType.Contain, value);
    }

    /// <summary>
    /// 设置查询条件信息。
    /// </summary>
    /// <param name="id">字段属性ID。</param>
    /// <param name="type">查询条件类型。</param>
    /// <param name="values">查询条件值。</param>
    /// <returns>查询条件信息。</returns>
    public QueryInfo SetQuery(string id, QueryType type, List<string> values)
    {
        return SetQuery(id, type, string.Join(",", values));
    }

    /// <summary>
    /// 设置查询条件信息。
    /// </summary>
    /// <param name="id">字段属性ID。</param>
    /// <param name="type">查询条件类型。</param>
    /// <param name="value">查询条件值。</param>
    /// <returns>查询条件信息。</returns>
    public QueryInfo SetQuery(string id, QueryType type, string value)
    {
        var query = Query?.FirstOrDefault(q => q.Id == id);
        if (query == null)
        {
            query = new QueryInfo(id, type, value);
            AddQuery(query);
        }

        query.Type = type;
        query.Value = value;
        query.ParamValue = value;
        return query;
    }

    /// <summary>
    /// 移除查询条件信息。
    /// </summary>
    /// <param name="id">字段属性ID。</param>
    public void RemoveQuery(string id)
    {
        var query = Query?.FirstOrDefault(q => q.Id == id);
        if (query != null)
        {
            Query.Remove(query);
        }
    }

    /// <summary>
    /// 将查询条件转换成数据库访问操作的参数字典。
    /// </summary>
    /// <param name="user">用户信息。</param>
    /// <returns>数据库访问操作的参数字典。</returns>
    public Dictionary<string, object> ToParameters(UserInfo user)
    {
        var parameter = new Dictionary<string, object>
        {
            [nameof(EntityBase.AppId)] = user?.AppId,
            [nameof(EntityBase.CompNo)] = user?.CompNo
        };

        if (Query != null && Query.Count > 0)
        {
            foreach (var item in Query)
            {
                if (string.IsNullOrWhiteSpace(item.Id))
                    continue;

                parameter[item.Id] = item.ParamValue;
            }
        }

        if (CmdParams != null && CmdParams.Count > 0)
        {
            foreach (var item in CmdParams)
            {
                if (parameter.ContainsKey(item.Key))
                    continue;

                parameter[item.Key] = item.Value;
            }
        }

        return parameter;
    }

    /// <summary>
    /// 判断是否有查询条件。
    /// </summary>
    /// <param name="id">字段属性ID。</param>
    /// <returns>返回是否有查询条件。</returns>
    public bool HasQuery(string id)
    {
        var query = Query?.FirstOrDefault(q => q.Id == id);
        if (query == null)
            return false;

        return !string.IsNullOrEmpty(query.Value);
    }

    /// <summary>
    /// 根据ID获取查询扩展参数泛型类型值。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="id">参数ID。</param>
    /// <returns>泛型类型值。</returns>
    public T GetParameter<T>(string id)
    {
        if (Parameters == null)
            return default;

        if (!Parameters.TryGetValue(id, out object value))
            return default;

        if (value == null)
            return default;

        if (typeof(T).IsClass && typeof(T) != typeof(string))
            return Utils.FromJson<T>(value.ToString());

        return Utils.ConvertTo<T>(value);
    }

    private void AddQuery(QueryInfo query)
    {
        Query ??= [];
        Query.Add(query);
    }
}