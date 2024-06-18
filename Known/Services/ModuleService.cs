namespace Known.Services;

public interface IModuleService : IService
{
    Task<List<SysModule>> GetModulesAsync();
    Task<Result> DeleteModulesAsync(List<SysModule> models);
    Task<Result> CopyModulesAsync(List<SysModule> models);
    Task<Result> MoveModulesAsync(List<SysModule> models);
    Task<Result> MoveModuleAsync(SysModule model);
    Task<Result> SaveModuleAsync(SysModule model);
}

class ModuleClient(HttpClient http) : ClientBase(http), IModuleService
{
    public Task<List<SysModule>> GetModulesAsync() => GetAsync<List<SysModule>>("Module/GetModules");
    public Task<Result> DeleteModulesAsync(List<SysModule> models) => PostAsync("Module/DeleteModules", models);
    public Task<Result> CopyModulesAsync(List<SysModule> models) => PostAsync("Module/CopyModules", models);
    public Task<Result> MoveModulesAsync(List<SysModule> models) => PostAsync("Module/MoveModules", models);
    public Task<Result> MoveModuleAsync(SysModule model) => PostAsync("Module/MoveModule", model);
    public Task<Result> SaveModuleAsync(SysModule model) => PostAsync("Module/SaveModule", model);
}

class ModuleService(Context context) : ServiceBase(context), IModuleService
{
    public Task<List<SysModule>> GetModulesAsync() => Database.QueryListAsync<SysModule>();

    public async Task<Result> DeleteModulesAsync(List<SysModule> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        foreach (var model in models)
        {
            if (await ModuleRepository.ExistsChildAsync(Database, model.Id))
                return Result.Error(Language["Tip.ModuleDeleteExistsChild"]);
        }

        return await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in models)
            {
                await db.DeleteAsync(item);
            }
        });
    }

    public async Task<Result> CopyModulesAsync(List<SysModule> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Copy, async db =>
        {
            foreach (var item in models)
            {
                await db.InsertAsync(item);
            }
        });
    }

    public async Task<Result> MoveModulesAsync(List<SysModule> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Save, async db =>
        {
            foreach (var item in models)
            {
                await db.SaveAsync(item);
            }
        });
    }

    public async Task<Result> MoveModuleAsync(SysModule model)
    {
        if (model == null)
            return Result.Error(Language.SelectOne);

        return await Database.TransactionAsync(Language.Save, async db =>
        {
            var sort = model.IsMoveUp ? model.Sort - 1 : model.Sort + 1;
            var module = await ModuleRepository.GetModuleAsync(db, model.ParentId, sort);
            if (module != null)
            {
                module.Sort = model.Sort;
                await db.SaveAsync(module);

                if (model.IsMoveUp)
                    model.Sort--;
                else
                    model.Sort++;
                await db.SaveAsync(model);
            }
        });
    }

    public async Task<Result> SaveModuleAsync(SysModule model)
    {
        var vr = model.Validate(Context);
        if (!vr.IsValid)
            return vr;

        if (string.IsNullOrWhiteSpace(model.Icon))
            model.Icon = "";//AntDesign不识别null值

        return await Database.TransactionAsync(Language.Save, async db =>
        {
            await db.SaveAsync(model);
        }, model);
    }
}