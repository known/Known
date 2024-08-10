namespace Known.Data;

class SqlServerBuilder : SqlBuilder
{
    public override string FormatName(string name) => $"[{name}]";

    protected override string GetTopSql(int size, string text)
    {
        return text.Replace("select", $"select top {size}");
    }
}