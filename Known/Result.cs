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
    public bool IsValid { get; set; } = true;

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
        IsValid = false;
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

    public PagingResult(int totalCount, List<T> pageData, object summary = null)
    {
        TotalCount = totalCount;
        PageData = pageData;
        Summary = summary;
    }

    public int TotalCount { get; set; }
    public List<T> PageData { get; set; }
    public Dictionary<string, object> Sums { get; set; }
    public object Summary { get; set; }
    public byte[] ExportData { get; set; }

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
    public List<ExportColumnInfo> ExportColumns { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
    public List<string> SumColumns { get; set; }

    public bool IsLoad { get; set; }
    public bool IsQuery { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public List<QueryInfo> Query { get; set; }
    public string[] OrderBys { get; set; }

    public Dictionary<string, string> Fields { get; set; }

    public void Clear()
    {
        IsLoad = true;
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

    internal Dictionary<string, object> ToParameters(UserInfo user)
    {
        var parameter = new Dictionary<string, object>
        {
            [nameof(EntityBase.AppId)] = user.AppId,
            [nameof(EntityBase.CompNo)] = user.CompNo
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

    internal bool HasQuery(string id)
    {
        if (Query == null)
            return false;

        var query = Query.FirstOrDefault(q => q.Id == id);
        if (query == null)
            return false;

        return !string.IsNullOrEmpty(query.Value);
    }

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
}

public class ExportColumnInfo
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public FieldType Type { get; set; }
}