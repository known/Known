namespace Known.Repositories;

class DictionaryRepository
{
    internal static Task<PagingResult<SysDictionary>> QueryDictionarysAsync(Database db, PagingCriteria criteria)
    {
        var sql = "select * from SysDictionary where AppId=@AppId and CompNo=@CompNo";
        return db.QueryPageAsync<SysDictionary>(sql, criteria);
    }

    internal static Task<List<SysDictionary>> GetDictionarysAsync(Database db, string appId, string compNo)
    {
        var sql = "select * from SysDictionary where AppId=@appId and CompNo=@compNo order by Category,Sort";
        return db.QueryListAsync<SysDictionary>(sql, new { appId, compNo });
    }
}