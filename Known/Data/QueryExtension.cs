namespace Known.Data;

public static class QueryExtension
{
    public static Task<PagingResult<T>> QueryPageAsync<T>(this Database db, PagingCriteria criteria)
    {
        var tableName = db.Builder.GetTableName<T>(true);
        var compNo = nameof(EntityBase.CompNo);
        var compName = db.Builder.FormatName(compNo);
        var sql = $"select * from {tableName} where {compName}=@{compNo}";
        return db.QueryPageAsync<T>(sql, criteria);
    }

    public static Task<T> QueryAsync<T>(this Database db, Expression<Func<T, bool>> expression)
    {
        var info = db.Builder.GetSelectCommand(expression);
        return db.QueryAsync<T>(info);
    }

    public static Task<List<T>> QueryListAsync<T>(this Database db, Expression<Func<T, bool>> expression)
    {
        var info = db.Builder.GetSelectCommand(expression);
        return db.QueryListAsync<T>(info);
    }

    public static Task<int> CountAsync<T>(this Database db, Expression<Func<T, bool>> expression)
    {
        var info = db.Builder.GetCountCommand(expression);
        return db.ScalarAsync<int>(info);
    }

    public static async Task<bool> ExistsAsync<T>(this Database db, Expression<Func<T, bool>> expression)
    {
        var info = db.Builder.GetCountCommand(expression);
        return await db.ScalarAsync<int>(info) > 0;
    }

    public static Task<int> DeleteAsync<T>(this Database db, Expression<Func<T, bool>> expression)
    {
        var info = db.Builder.GetDeleteCommand(expression);
        return db.ExecuteNonQueryAsync(info);
    }

    public static QueryBuilder<T> Query<T>(this Database db) => new(db);
    public static QueryBuilder<T> Select<T>(this Database db) => new QueryBuilder<T>(db).Select();
}