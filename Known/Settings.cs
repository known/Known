using System.ComponentModel;

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
            var codes = Cache.GetCodes(column.Category, false);
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

    [DisplayName("暗亮模式")]
    public bool IsLight { get; set; }

    [DisplayName("手风琴菜单")]
    public bool Accordion { get; set; }

    [DisplayName("标签页")]
    public bool MultiTab { get; set; }
}