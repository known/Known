namespace Known.Data;

class SqlBuilder(DbProvider provider)
{
    private readonly List<string> sqls = [];

    public SqlBuilder SelectAll()
    {
        sqls.Add("select *");
        return this;
    }

    public SqlBuilder SelectCount()
    {
        sqls.Add("select count(*)");
        return this;
    }

    public SqlBuilder From<T>()
    {
        var tableName = provider?.GetTableName(typeof(T));
        tableName = provider?.FormatName(tableName);
        sqls.Add($"from {tableName}");
        return this;
    }

    public SqlBuilder Where(string field)
    {
        var column = provider?.FormatName(field);
        sqls.Add($"where {column}=@{field}");
        return this;
    }

    public SqlBuilder WhereSql(string where)
    {
        sqls.Add($"where {where}");
        return this;
    }

    public SqlBuilder OrderBy(string field, string order = null)
    {
        var column = provider?.FormatName(field);
        sqls.Add($"order by {column}{order}");
        return this;
    }

    public string ToSqlString()
    {
        return string.Join(" ", sqls);
    }
}