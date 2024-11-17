namespace Known.Extensions;

static class ConfigExtension
{
    internal static async Task<string> GetConfigAsync(this Database db, string key)
    {
        var appId = Config.App.Id;
        var config = await db.QueryAsync<SysConfig>(d => d.AppId == appId && d.ConfigKey == key);
        return config?.ConfigValue;
    }

    internal static async Task<T> GetConfigAsync<T>(this Database db, string key)
    {
        var json = await db.GetConfigAsync(key);
        return Utils.FromJson<T>(json);
    }

    internal static async Task SaveConfigAsync(this Database db, string key, object value)
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
}