namespace Known.Core.Repositories;

class PlatformRepository
{
    internal static string GetConfig(Database db, string appId, string key)
    {
        var sql = "select ConfigValue from SysConfig where AppId=@appId and ConfigKey=@key";
        return db.Scalar<string>(sql, new { appId, key });
    }

    internal static void SaveConfig(Database db, string appId, string key, string value)
    {
        var sql = "select count(*) from SysConfig where AppId=@appId and ConfigKey=@key";
        if (db.Scalar<int>(sql, new { appId, key }) > 0)
        {
            sql = "update SysConfig set ConfigValue=@value where AppId=@appId and ConfigKey=@key";
            db.Execute(sql, new { value, appId, key });
        }
        else
        {
            sql = "insert into SysConfig(AppId,ConfigKey,ConfigValue) values(@appId,@key,@value)";
            db.Execute(sql, new { appId, key, value });
        }
    }
}