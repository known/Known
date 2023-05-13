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
    public QueryInfo(string id, string value)
    {
        Id = id;
        Value = value;
        Type = QueryType.Contain;
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
    public string Theme { get; set; } = "Default";
    public bool MultiTab { get; set; }
    public int PageSize { get; set; } = PagingCriteria.DefaultPageSize; 
}