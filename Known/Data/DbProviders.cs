namespace Known.Data;

class AccessProvider : DbProvider
{
    public override string FormatName(string name) => $"`{name}`";

    internal override string GetTopSql(int size, string text)
    {
        return $"select top {size} t1.* from ({text}) t1";
    }

    internal override string GetPageSql(string text, string order, PagingCriteria criteria)
    {
        //var order = string.Empty;
        //if (criteria.OrderBys != null)
        //{
        //    order = string.Join(",", criteria.OrderBys);
        //    if (criteria.OrderBys.Length > 1)
        //        return $"{text} order by {order}";
        //}
        //else
        //{
        //    order = "CreateTime";
        //}

        var order1 = $"{order} desc";
        if (order.EndsWith("desc"))
            order1 = order.Replace("desc", "");
        else if (order.EndsWith("asc"))
            order1 = order.Replace("asc", "desc");

        var page = criteria.PageIndex;
        return $@"select t3.* from (
    select top {criteria.PageSize} t2.* from(
        select top {page * criteria.PageSize} t1.* from ({text}) t1 order by t1.{order}
    ) t2 order by t2.{order1}
) t3 order by t3.{order}";
    }
}

class SQLiteProvider : DbProvider
{
    internal override string GetTopSql(int size, string text)
    {
        return $"{text} limit {size} offset 0";
    }

    internal override string GetPageSql(string text, string order, PagingCriteria criteria)
    {
        var startNo = criteria.PageSize * (criteria.PageIndex - 1);
        return $"{text} order by {order} limit {criteria.PageSize} offset {startNo}";
    }
}

class SqlServerProvider : DbProvider
{
    public override string FormatName(string name) => $"[{name}]";

    internal override string GetTopSql(int size, string text)
    {
        return text.Replace("select", $"select top {size}");
    }
}

class OracleProvider : DbProvider
{
    public override string Prefix => ":";

    public override string GetDateSql(string name, bool withTime = true)
    {
        var format = "yyyy-mm-dd";
        if (withTime)
            format += " hh24:mi:ss";
        return $"to_date(:{name},'{format}')";
    }
}

class MySqlProvider : DbProvider
{
    internal override string GetTopSql(int size, string text) => $"{text} limit 0, {size}";

    internal override string GetPageSql(string text, string order, PagingCriteria criteria)
    {
        var startNo = criteria.PageSize * (criteria.PageIndex - 1);
        return $"{text} order by {order} limit {startNo}, {criteria.PageSize}";
    }
}

class PgSqlProvider : DbProvider
{
    //public override string FormatName(string name) => $"\"{name}\"";
    public override object FormatDate(string date) => DateTime.Parse(date);
}

class DMProvider : DbProvider
{
    public override string Prefix => ":";

    public override string FormatName(string name) => $"\"{name}\"";
}