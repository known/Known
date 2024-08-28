namespace Known.Data;

class SQLiteProvider : DbProvider
{
    protected override string GetPageSql(string text, string order, PagingCriteria criteria)
    {
        var startNo = criteria.PageSize * (criteria.PageIndex - 1);
        return $"{text} order by {order} limit {criteria.PageSize} offset {startNo}";
    }
}