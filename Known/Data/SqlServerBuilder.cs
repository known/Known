namespace Known.Data;

class SqlServerBuilder : SqlBuilder
{
    protected override string GetTopSql(int size, string text)
    {
        return text.Replace("select", $"select top {size}");
    }
}