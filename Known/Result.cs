namespace Known;

/// <summary>
/// 验证结果类。
/// </summary>
public class Result
{
    private readonly List<string> errors = [];
    private string message;

    /// <summary>
    /// 构造函数，创建一个验证结果类的实例。
    /// </summary>
    public Result()
    {
        errors.Clear();
    }

    private Result(string message, object data) : this()
    {
        this.message = message;
        Data = data;
    }

    /// <summary>
    /// 取得或设置操作成功后是否关闭对话框，默认关闭。
    /// </summary>
    public bool IsClose { get; set; } = true;

    /// <summary>
    /// 取得或设置操作是否成功。
    /// </summary>
    public bool IsValid { get; set; } = true;

    /// <summary>
    /// 取得或设置操作成功或失败提示消息。
    /// </summary>
    public string Message
    {
        get
        {
            if (errors.Count == 0)
                return message;

            return string.Join(Environment.NewLine, [.. errors]);
        }
        set { message = value; }
    }

    /// <summary>
    /// 取得或设置操作返回的扩展对象。
    /// </summary>
    public object Data { get; set; }

    /// <summary>
    /// 获取返回的扩展泛型对象。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <returns>泛型对象。</returns>
    public T DataAs<T>()
    {
        if (Data == null)
            return default;

        if (Data is T data)
            return data;

        var dataString = Data.ToString();
        return Utils.FromJson<T>(dataString);
    }

    /// <summary>
    /// 添加错误信息。
    /// </summary>
    /// <param name="message">错误信息。</param>
    public void AddError(string message)
    {
        IsValid = false;
        errors.Add(message);
    }

    /// <summary>
    /// 验证条件是否通过，不通过则添加错误信息。
    /// </summary>
    /// <param name="broken">验证条件。</param>
    /// <param name="message">错误信息。</param>
    public void Validate(bool broken, string message)
    {
        if (broken)
        {
            AddError(message);
        }
    }

    /// <summary>
    /// 添加必填错误信息。
    /// </summary>
    /// <param name="context">上下文对象。</param>
    /// <param name="name">语言名称。</param>
    /// <param name="value">校验该值是否为空。</param>
    public void Required(Context context, string name, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            AddError(context.Language.Required(name));
    }

    /// <summary>
    /// 返回失败结果。
    /// </summary>
    /// <param name="message">错误信息。</param>
    /// <param name="data">扩展数据，默认为空。</param>
    /// <returns>失败结果。</returns>
    public static Result Error(string message, object data = null)
    {
        var result = new Result("", data);
        result.AddError(message);
        return result;
    }

    /// <summary>
    /// 异步返回失败结果。
    /// </summary>
    /// <param name="message">错误信息。</param>
    /// <param name="data">扩展数据，默认为空。</param>
    /// <returns>失败结果。</returns>
    public static Task<Result> ErrorAsync(string message, object data = null) => Task.FromResult(Error(message, data));

    /// <summary>
    /// 返回成功结果。
    /// </summary>
    /// <param name="message">成功信息。</param>
    /// <param name="data">扩展数据，默认为空。</param>
    /// <returns>成功结果。</returns>
    public static Result Success(string message, object data = null) => new(message, data);

    /// <summary>
    /// 异步返回成功结果。
    /// </summary>
    /// <param name="message">成功信息。</param>
    /// <param name="data">扩展数据，默认为空。</param>
    /// <returns>成功结果。</returns>
    public static Task<Result> SuccessAsync(string message, object data = null)
    {
        return Task.FromResult(Success(message, data));
    }
}

/// <summary>
/// 泛型数据分页查询结果类。
/// </summary>
/// <typeparam name="T">泛型类型。</typeparam>
public class PagingResult<T>
{
    /// <summary>
    /// 构造函数，创建一个泛型数据分页查询结果类的实例。
    /// </summary>
    public PagingResult() { }

    /// <summary>
    /// 构造函数，创建一个泛型数据分页查询结果类的实例。
    /// </summary>
    /// <param name="pageData">分页数据列表。</param>
    public PagingResult(List<T> pageData)
    {
        TotalCount = pageData?.Count ?? 0;
        PageData = pageData;
    }

    /// <summary>
    /// 构造函数，创建一个泛型数据分页查询结果类的实例。
    /// </summary>
    /// <param name="totalCount">总记录数。</param>
    /// <param name="pageData">分页数据列表。</param>
    /// <param name="summary">统计摘要对象。</param>
    public PagingResult(int totalCount, List<T> pageData, object summary = null)
    {
        TotalCount = totalCount;
        PageData = pageData;
        Summary = summary;
    }

    /// <summary>
    /// 取得或设置总记录数。
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// 取得或设置分页数据列表。
    /// </summary>
    public List<T> PageData { get; set; }

    /// <summary>
    /// 取得或设置统计字段字典。
    /// </summary>
    public Dictionary<string, object> Statis { get; set; }

    /// <summary>
    /// 取得或设置统计摘要对象。
    /// </summary>
    public object Summary { get; set; }

    /// <summary>
    /// 取得或设置导出数据字节数组。
    /// </summary>
    public byte[] ExportData { get; set; }

    /// <summary>
    /// 获取泛型统计摘要对象。
    /// </summary>
    /// <typeparam name="TSummary">摘要泛型类型。</typeparam>
    /// <returns>泛型统计摘要对象。</returns>
    public TSummary SummaryAs<TSummary>()
    {
        if (Summary == null)
            return default;

        if (Summary is TSummary data)
            return data;

        var dataString = Summary.ToString();
        return Utils.FromJson<TSummary>(dataString);
    }
}

/// <summary>
/// 导出模式枚举。
/// </summary>
public enum ExportMode
{
    /// <summary>
    /// 不导出。
    /// </summary>
    None,
    /// <summary>
    /// 导出当前页数据。
    /// </summary>
    Page,
    /// <summary>
    /// 导出查询结果数据。
    /// </summary>
    Query,
    /// <summary>
    /// 导出全部数据。
    /// </summary>
    All
}

/// <summary>
/// 查询条件操作类型枚举。
/// </summary>
public enum QueryType
{
    /// <summary>
    /// 等于。
    /// </summary>
    Equal,
    /// <summary>
    /// 不等于。
    /// </summary>
    NotEqual,
    /// <summary>
    /// 小于。
    /// </summary>
    LessThan,
    /// <summary>
    /// 小于等于。
    /// </summary>
    LessEqual,
    /// <summary>
    /// 大于。
    /// </summary>
    GreatThan,
    /// <summary>
    /// 大于等于。
    /// </summary>
    GreatEqual,
    /// <summary>
    /// 区间(含两者)。
    /// </summary>
    Between,
    /// <summary>
    /// 区间(不含两者)。
    /// </summary>
    BetweenNotEqual,
    /// <summary>
    /// 区间(仅含前者)。
    /// </summary>
    BetweenLessEqual,
    /// <summary>
    /// 区间(仅含后者)。
    /// </summary>
    BetweenGreatEqual,
    /// <summary>
    /// 包含于。
    /// </summary>
    Contain,
    /// <summary>
    /// 不包含于。
    /// </summary>
    NotContain,
    /// <summary>
    /// 开头于。
    /// </summary>
    StartWith,
    /// <summary>
    /// 不开头于。
    /// </summary>
    NotStartWith,
    /// <summary>
    /// 结尾于。
    /// </summary>
    EndWith,
    /// <summary>
    /// 不结尾于。
    /// </summary>
    NotEndWith,
    /// <summary>
    /// 批量(逗号分割)。
    /// </summary>
    Batch
}

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
    public List<StatisColumnInfo> StatisColumns { get; set; }

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
        StatisColumns = [];
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
        var query = Query?.FirstOrDefault(q => q.Id == id);
        if (query == null)
        {
            query = new QueryInfo(id, value);
            AddQuery(query);
        }

        query.Value = value;
        query.ParamValue = value;
        return query;
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

/// <summary>
/// 导出栏位信息类。
/// </summary>
public class ExportColumnInfo
{
    /// <summary>
    /// 取得或设置导出栏位ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置导出栏位名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置导出栏位代码表类别名。
    /// </summary>
    public string Category { get; set; }

    /// <summary>
    /// 取得或设置导出栏位字段类型。
    /// </summary>
    public FieldType Type { get; set; }

    /// <summary>
    /// 取得或设置导出栏位是否是附加栏位。
    /// </summary>
    public bool IsAdditional { get; set; }
}

/// <summary>
/// 统计栏位信息类。
/// </summary>
public class StatisColumnInfo
{
    /// <summary>
    /// 取得或设置统计栏位ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置统计栏位功能方法，默认为sum。
    /// </summary>
    public string Function { get; set; } = "sum";

    /// <summary>
    /// 取得或设置统计栏位SQL语句表达式。
    /// </summary>
    public string Expression { get; set; }
}