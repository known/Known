namespace Known.Services;

partial class AdminService
{
    public Task<string> GetConfigAsync(string key)
    {
        return Database.GetConfigAsync(key);
    }

    public Task<Result> SaveConfigAsync(ConfigInfo info)
    {
        return Database.SaveConfigAsync(info.Key, info.Value);
    }
}