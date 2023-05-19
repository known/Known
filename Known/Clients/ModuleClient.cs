namespace Known.Clients;

public class ModuleClient : ClientBase
{
    public ModuleClient(Context context) : base(context) { }

    public Task<List<SysModule>> GetModulesAsync() => Context.GetAsync<List<SysModule>>("Module/GetModules");
    public Task<Result> DeleteModulesAsync(List<SysModule> models) => Context.PostAsync("Module/DeleteModules", models);
    public Task<Result> CopyModulesAsync(List<SysModule> models) => Context.PostAsync("Module/CopyModules", models);
    public Task<Result> MoveModulesAsync(List<SysModule> models) => Context.PostAsync("Module/MoveModules", models);
    public Task<Result> MoveModuleAsync(SysModule model) => Context.PostAsync("Module/MoveModule", model);
    public Task<Result> SaveModuleAsync(object model) => Context.PostAsync("Module/SaveModule", model);
}