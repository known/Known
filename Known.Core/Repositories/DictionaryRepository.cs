namespace Known.Core.Repositories;

class DictionaryRepository
{
    internal static PagingResult<SysDictionary> QueryDictionarys(Database db, PagingCriteria criteria)
    {
        var sql = "select * from SysDictionary where AppId=@AppId and CompNo=@CompNo";
        return db.QueryPage<SysDictionary>(sql, criteria);
    }

    internal static List<SysDictionary> GetDictionarys(Database db, string appId, string compNo)
    {
        var sql = "select * from SysDictionary where AppId=@appId and CompNo=@compNo order by Category,Sort";
        return db.QueryList<SysDictionary>(sql, new { appId, compNo });
    }
}