namespace Known.Clients;

public class SystemClient : ClientBase
{
    public SystemClient(Context context) : base(context) { }

    //Config
    public async Task<T> GetConfigAsync<T>(string key)
    {
        var json = await Context.GetAsync($"System/GetConfig?key={key}");
        return Utils.FromJson<T>(json);
    }

    public Task<Result> SaveConfigAsync(ConfigInfo info) => Context.PostAsync("System/SaveConfig", info);

    //Install
    public Task<Result> CheckInstallAsync() => Context.PostAsync("System/CheckInstall");
    public Task<Result> UpdateKeyAsync(InstallInfo info) => Context.PostAsync("System/UpdateKey", info);
    public Task<Result> SaveInstallAsync(InstallInfo info) => Context.PostAsync("System/SaveInstall", info);

    //System
    public Task<SystemInfo> GetSystemAsync() => Context.GetAsync<SystemInfo>("System/GetSystem");
    public Task<Result> SaveKeyAsync(SystemInfo info) => Context.PostAsync("System/SaveKey", info);
    public Task<Result> SaveSystemAsync(SystemInfo info) => Context.PostAsync("System/SaveSystem", info);
    public Task<Result> SaveSystemConfigAsync(SystemInfo info) => Context.PostAsync("System/SaveSystemConfig", info);

    //Tenant
    public Task<PagingResult<SysTenant>> QueryTenantsAsync(PagingCriteria criteria) => Context.QueryAsync<SysTenant>("System/QueryTenants", criteria);
    public Task<Result> SaveTenantAsync(object model) => Context.PostAsync("System/SaveTenant", model);
}