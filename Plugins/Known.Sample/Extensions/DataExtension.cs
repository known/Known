namespace Known.Sample.Extensions;

static class DataExtension
{
    internal static Task<string> GetMaxWorkNoAsync(this Database db)
    {
        var prefix = $"W{DateTime.Now:yyMMdd}";
        return db.GetMaxFormNoAsync<TbWork>(nameof(TbWork.WorkNo), prefix, 3);
    }

    private static async Task<string> GetMaxFormNoAsync<T>(this Database db, string field, string prefix, int length)
    {
        var tableName = typeof(T).Name;
        var sql = $"select max({field}) from {tableName} where CompNo=@CompNo and {field} like '{prefix}%'";
        var maxNo = await db.ScalarAsync<string>(sql, new { db.User.CompNo });
        if (string.IsNullOrWhiteSpace(maxNo) || maxNo == prefix)
            maxNo = prefix + new string('0', length);
        return Utils.GetMaxFormNo(prefix, maxNo);
    }
}