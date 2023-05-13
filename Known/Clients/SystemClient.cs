namespace Known.Clients;

public class SystemClient : BaseClient
{
    public SystemClient(Context context) : base(context) { }

    public async Task<T> GetConfigAsync<T>(string key)
    {
        var json = await Context.GetAsync($"System/GetConfig?key={key}");
        return Utils.FromJson<T>(json);
    }

    public Task<Result> SaveConfigAsync(ConfigInfo info) => Context.PostAsync("System/SaveConfig", info);
    public Task<Result> CheckInstallAsync() => Context.PostAsync("System/CheckInstall");
    public Task<Result> UpdateKeyAsync(InstallInfo info) => Context.PostAsync("System/UpdateKey", info);
    public Task<Result> SaveInstallAsync(InstallInfo info) => Context.PostAsync("System/SaveInstall", info);
    public Task<SystemInfo> GetSystemAsync() => Context.GetAsync<SystemInfo>("System/GetSystem");
    public Task<Result> SaveSystemAsync(SystemInfo info) => Context.PostAsync("System/SaveSystem", info);
}