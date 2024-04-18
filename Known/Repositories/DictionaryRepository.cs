namespace Known.Repositories;

class DictionaryRepository
{
    internal static Task<List<SysDictionary>> GetCategoriesAsync(Database db)
    {
        var sql = $"select Category,Code,Name from SysDictionary where Enabled='True' and CompNo=@CompNo and Category='{Constants.DicCategory}' order by Sort";
        return db.QueryListAsync<SysDictionary>(sql, new { db.User.CompNo });
    }

    internal static Task<PagingResult<SysDictionary>> QueryDictionarysAsync(Database db, PagingCriteria criteria)
    {
        var sql = "select * from SysDictionary where CompNo=@CompNo";
        return db.QueryPageAsync<SysDictionary>(sql, criteria);
    }

    internal static Task<List<SysDictionary>> GetDictionarysAsync(Database db)
    {
        var sql = "select * from SysDictionary where Enabled='True' order by Category,Sort";
        return db.QueryListAsync<SysDictionary>(sql);
    }

    internal static async Task<bool> ExistsDictionary(Database db, SysDictionary dictionary)
    {
        var sql = "select count(*) from SysDictionary where CompNo=@CompNo and Category=@Category and Code=@Code and Id<>@Id";
        var count = await db.ScalarAsync<int>(sql, dictionary);
        return count > 0;
    }

    internal static Task DeleteDictionarysAsync(Database db, string category)
    {
        var sql = "delete from SysDictionary where CompNo=@CompNo and Category=@category";
        return db.ExecuteAsync(sql, new { db.User.CompNo, category });
    }
}