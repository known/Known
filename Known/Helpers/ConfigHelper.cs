namespace Known.Helpers;

public sealed class ConfigHelper
{
    private ConfigHelper() { }

    private const string KeySystem = "SystemInfo";

    public static async Task<T> GetConfigAsync<T>(Database db, string key)
    {
        var json = await GetConfigAsync(db, key);
        return Utils.FromJson<T>(json);
    }

    public static async Task<string> GetConfigAsync(Database db, string key)
    {
        var appId = Config.App.Id;
        var config = await db.QueryAsync<SysConfig>(d => d.AppId == appId && d.ConfigKey == key);
        return config?.ConfigValue;
    }

    public static async Task SaveConfigAsync(Database db, string key, object value)
    {
        var appId = Config.App.Id;
        var data = new Dictionary<string, object>();
        data[nameof(SysConfig.AppId)] = appId;
        data[nameof(SysConfig.ConfigKey)] = key;
        data[nameof(SysConfig.ConfigValue)] = Utils.ToJson(value);
        var scalar = await db.CountAsync<SysConfig>(d => d.AppId == appId && d.ConfigKey == key);
        if (scalar > 0)
            await db.UpdateAsync(nameof(SysConfig), "AppId,ConfigKey", data);
        else
            await db.InsertAsync(nameof(SysConfig), data);
    }

    public static async Task<SystemInfo> GetSystemAsync(Database db)
    {
        if (!Config.App.IsPlatform || db.User == null)
        {
            var json = await GetConfigAsync(db, KeySystem);
            return Utils.FromJson<SystemInfo>(json);
        }

        var company = await db.QueryAsync<SysCompany>(d => d.Code == db.User.CompNo);
        if (company == null)
            return GetSystem(db.User);

        return Utils.FromJson<SystemInfo>(company.SystemData);
    }

    public static Task SaveSystemAsync(Database db, SystemInfo info)
    {
        return SaveConfigAsync(db, KeySystem, info);
    }

    private static SystemInfo GetSystem(UserInfo info)
    {
        return new SystemInfo
        {
            CompNo = info?.CompNo,
            CompName = info?.CompName,
            AppName = Config.App.Name
        };
    }
}