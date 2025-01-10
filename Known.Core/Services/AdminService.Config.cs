namespace Known.Services;

partial class AdminService
{
    [AllowAnonymous]
    public async Task<bool> GetIsInstallAsync()
    {
        var db = Database;
        db.EnableLog = false;
        Config.System ??= await db.GetSystemAsync();
        return Config.System == null;
    }

    public Task<string> GetConfigAsync(string key)
    {
        return Database.GetConfigAsync(key);
    }

    public Task<Result> SaveConfigAsync(ConfigInfo info)
    {
        return Database.SaveConfigAsync(info.Key, info.Value);
    }
}