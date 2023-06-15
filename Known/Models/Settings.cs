namespace Known.Models;

public class UserSetting
{
    public const string KeyInfo = "UserSetting";
    public const string KeyQuery = "UserQuery";
    public const string KeyColumn = "UserColumn";

    public SettingInfo Info { get; set; }
    public Dictionary<string, List<QueryInfo>> Querys { get; set; }
    public Dictionary<string, List<ColumnInfo>> Columns { get; set; }
}

public class QueryInfo
{
    public QueryInfo() { }
    public QueryInfo(string id, string value) : this(id, QueryType.Contain, value) { }
    public QueryInfo(string id, QueryType type, string value)
    {
        Id = id;
        Type = type;
        Value = value;
    }

    public string Id { get; set; }
    public string Value { get; set; }
    public QueryType Type { get; set; }
}

public class SettingInfo
{
    public static SettingInfo Default
    {
        get { return new SettingInfo(); }
    }

    public string Language { get; set; }
    public string ThemeColor { get; set; } = "#1c66b9";
    public bool RandomColor { get; set; }
    public bool MultiTab { get; set; }
    public int PageSize { get; set; } = PagingCriteria.DefaultPageSize; 
}