namespace Known;

public class QueryInfo
{
    public QueryInfo(string id, string value) : this(id, QueryType.Contain, value) { }
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

    public string Id { get; set; }
    public string Value { get; set; }
    public QueryType Type { get; set; }
    internal string ParamValue { get; set; }
}

public class SettingInfo
{
    internal const string KeyInfo = "UserSetting";

    public SettingInfo()
    {
        Accordion = true;
    }

    [Form] public bool IsLight { get; set; }
    [Form] public bool Accordion { get; set; }
    [Form] public bool MultiTab { get; set; }
}