namespace Known;

public class Result
{
    private readonly List<string> errors = [];
    private string message;

    public Result()
    {
        errors.Clear();
    }

    private Result(string message, object data) : this()
    {
        this.message = message;
        Data = data;
    }

    public bool IsClose { get; set; } = true;
    public bool IsValid => errors.Count == 0;

    public string Message
    {
        get
        {
            if (errors.Count == 0)
                return message;

            return string.Join(Environment.NewLine, errors.ToArray());
        }
        internal set { message = value; }
    }

    public object Data { get; set; }

    public T DataAs<T>()
    {
        if (Data == null)
            return default;

        if (Data is T data)
            return data;

        var dataString = Data.ToString();
        return Utils.FromJson<T>(dataString);
    }

    public void AddError(string message)
    {
        errors.Add(message);
    }

    public void Validate(bool broken, string message)
    {
        if (broken)
        {
            AddError(message);
        }
    }

    public void Required(Context context, string name, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            AddError(context.Language.Required(name));
    }

    public static Result Error(string message, object data = null)
    {
        var result = new Result("", data);
        result.AddError(message);
        return result;
    }

    public static Task<Result> ErrorAsync(string message, object data = null) => Task.FromResult(Error(message, data));
    public static Result Success(string message, object data = null) => new(message, data);
    public static Task<Result> SuccessAsync(string message, object data = null) => Task.FromResult(Success(message, data));
}

public class PagingResult<T>
{
    public PagingResult() { }

    public PagingResult(List<T> pageData)
    {
        TotalCount = pageData?.Count ?? 0;
        PageData = pageData;
    }

    public PagingResult(int totalCount, List<T> pageData)
    {
        TotalCount = totalCount;
        PageData = pageData;
    }

    public int TotalCount { get; }
    public List<T> PageData { get; }
    public Dictionary<string, object> Sums { get; internal set; }
    public object Summary { get; internal set; }
    public byte[] ExportData { get; internal set; }

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

public enum ExportMode { None, Page, Query, All }
public enum QueryType
{
    Equal,
    NotEqual,
    LessThan,
    LessEqual,
    GreatThan,
    GreatEqual,
    Between,
    BetweenNotEqual,
    BetweenLessEqual,
    BetweenGreatEqual,
    Contain,
    StartWith,
    EndWith,
    Batch
}

public class PagingCriteria
{
    public PagingCriteria()
    {
        PageSize = Config.App.DefaultPageSize;
        Clear();
    }

    public ExportMode ExportMode { get; set; }
    public Dictionary<string, string> ExportColumns { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
    public List<string> SumColumns { get; set; }

    public bool IsQuery { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public List<QueryInfo> Query { get; set; }
    public string[] OrderBys { get; set; }

    internal Dictionary<string, string> Fields { get; set; }

    internal void Clear()
    {
        IsQuery = false;
        ExportColumns = [];
        Parameters = [];
        SumColumns = [];
        Query = [];
        OrderBys = [];
        Fields = [];
        PageIndex = 1;
    }

    public QueryInfo SetQuery(string id, string value)
    {
        var query = Query.FirstOrDefault(q => q.Id == id);
        if (query == null)
        {
            query = new QueryInfo(id, value);
            Query.Add(query);
        }

        query.Value = value;
        query.ParamValue = value;
        return query;
    }

    public QueryInfo SetQuery(string id, QueryType type, string value)
    {
        var query = Query.FirstOrDefault(q => q.Id == id);
        if (query == null)
        {
            query = new QueryInfo(id, type, value);
            Query.Add(query);
        }

        query.Type = type;
        query.Value = value;
        query.ParamValue = value;
        return query;
    }

    public void RemoveQuery(string id)
    {
        var query = Query.FirstOrDefault(q => q.Id == id);
        if (query != null)
        {
            Query.Remove(query);
        }
    }

    internal Dictionary<string, string> ToParameters(UserInfo user)
    {
        var parameter = new Dictionary<string, string>
        {
            ["AppId"] = user.AppId,
            ["CompNo"] = user.CompNo
        };

        if (Query != null && Query.Count > 0)
        {
            foreach (var item in Query)
            {
                parameter[item.Id] = item.ParamValue;
            }
        }
        return parameter;
    }

    public string GetQueryValue(string id)
    {
        if (Query == null)
            return string.Empty;

        var query = Query.FirstOrDefault(q => q.Id == id);
        if (query == null)
            return string.Empty;

        return query.Value;
    }

    internal bool HasQuery(string id)
    {
        if (Query == null)
            return false;

        var query = Query.FirstOrDefault(q => q.Id == id);
        if (query == null)
            return false;

        return !string.IsNullOrEmpty(query.Value);
    }

    public string GetParameter(string id)
    {
        if (Parameters == null)
            return string.Empty;

        if (!Parameters.TryGetValue(id, out object value))
            return string.Empty;

        if (value == null)
            return string.Empty;

        return value.ToString();
    }

    public T GetParameter<T>(string id)
    {
        if (Parameters == null)
            return default;

        if (!Parameters.TryGetValue(id, out object value))
            return default;

        if (value == null)
            return default;

        return (T)value;
    }
}