using System.ComponentModel;

namespace Known;

public class Result
{
    private readonly List<string> errors = new();
    private string message;

    public Result()
    {
        errors.Clear();
        Size = null;
        IsValid = true;
    }

    private Result(string message, object data) : this()
    {
        this.message = message;
        Data = data;
    }

    public bool IsClose { get; set; } = true;
    public bool IsValid { get; set; }
    public Size? Size { get; set; }

    public string Message
    {
        get
        {
            if (errors.Count == 0)
                return message;

            return string.Join(Environment.NewLine, errors.ToArray());
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

    public void Required(string name, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            AddError(Language.NotEmpty.Format(name));
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
    public int TotalCount { get; set; }
    public List<T> PageData { get; set; }
    public Dictionary<string, object> Sums { get; set; }
    public object Summary { get; set; }
    public byte[] ExportData { get; set; }

    public TSummary SummaryAs<TSummary>()
    {
        if (Summary == null)
            return default;

        var dataString = Summary.ToString();
        return Utils.FromJson<TSummary>(dataString);
    }
}

public enum ExportMode { None, Page, Query, All }
public enum QueryType
{
    [Description("等于")] Equal,
    [Description("不等于")] NotEqual,
    [Description("小于")] LessThan,
    [Description("小于等于")] LessEqual,
    [Description("大于")] GreatThan,
    [Description("大于等于")] GreatEqual,
    [Description("两者之间(含两者)")] Between,
    [Description("两者之间(不含两者)")] BetweenNotEqual,
    [Description("两者之间(仅含前者)")] BetweenLessEqual,
    [Description("两者之间(仅含后者)")] BetweenGreatEqual,
    [Description("包含于")] Contain,
    [Description("开头于")] StartWith,
    [Description("结尾于")] EndWith,
    [Description("批量(逗号分割)")] Batch
}

public class PagingCriteria
{
    public PagingCriteria()
    {
        Parameters = [];
        Query = [];
        Fields = [];
    }

    public PagingCriteria(int pageIndex) : this()
    {
        PageIndex = pageIndex;
    }

    public static int[] PageSizes { get; set; } = [10, 15, 20, 25, 30, 40, 50, 100, 200, 500, 1000, 2000];
    public static int DefaultPageSize { get; set; } = 10;

    public ExportMode ExportMode { get; set; }
    public string ExportExtension { get; set; }
    public Dictionary<string, string> ExportColumns { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
    public List<string> SumColumns { get; set; }

    public bool IsQuery { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; } = DefaultPageSize;
    public List<QueryInfo> Query { get; set; }
    public string[] OrderBys { get; set; }

    internal Dictionary<string, string> Fields { get; }

    public void SetQuery(string id, string value)
    {
        var query = Query.FirstOrDefault(q => q.Id == id);
        if (query == null)
        {
            Query.Add(new(id, value));
            return;
        }

        query.Value = value;
    }

    public void SetQuery(string id, QueryType type, string value)
    {
        var query = Query.FirstOrDefault(q => q.Id == id);
        if (query == null)
        {
            Query.Add(new(id, type, value));
            return;
        }

        query.Type = type;
        query.Value = value;
    }
}