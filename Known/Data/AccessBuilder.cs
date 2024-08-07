namespace Known.Data;

class AccessBuilder : SqlBuilder
{
    public override string FormatName(string name)
    {
        return $"`{name}`";
    }

    protected override string GetPageSql(string text, string order, PagingCriteria criteria)
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