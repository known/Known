namespace Known.Data;

class MySqlBuilder : SqlBuilder
{
    protected override string GetTopSql(int size, string text) => $"{text} limit 0, {size}";

    protected override string GetPageSql(string text, string order, PagingCriteria criteria)
    {
        var startNo = criteria.PageSize * (criteria.PageIndex - 1);
        return $"{text} order by {order} limit {startNo}, {criteria.PageSize}";
    }
}