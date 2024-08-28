namespace Known.Data;

class SqlServerProvider : DbProvider
{
    public override string FormatName(string name) => $"[{name}]";

    protected override string GetTopSql(int size, string text)
    {
        return text.Replace("select", $"select top {size}");
    }
}