namespace Known.Repositories;

class PlatformRepository
{
    internal static Task<string> GetConfigAsync(Database db, string appId, string key)
    {
        var sql = "select ConfigValue from SysConfig where AppId=@appId and ConfigKey=@key";
        return db.ScalarAsync<string>(sql, new { appId, key });
    }

    internal static async Task<int> SaveConfigAsync(Database db, string appId, string key, string value)
    {
        var sql = "select count(*) from SysConfig where AppId=@appId and ConfigKey=@key";
        var scalar = await db.ScalarAsync<int>(sql, new { appId, key });
        if (scalar > 0)
        {
            sql = "update SysConfig set ConfigValue=@value where AppId=@appId and ConfigKey=@key";
            return await db.ExecuteAsync(sql, new { value, appId, key });
        }
        else
        {
            sql = "insert into SysConfig(AppId,ConfigKey,ConfigValue) values(@appId,@key,@value)";
            return await db.ExecuteAsync(sql, new { appId, key, value });
        }
    }
}