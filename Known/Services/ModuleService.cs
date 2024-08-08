namespace Known.Services;

public interface IModuleService : IService
{
    Task<List<SysModule>> GetModulesAsync();
    Task<SysModule> GetModuleAsync(string id);
    Task<Result> DeleteModulesAsync(List<SysModule> models);
    Task<Result> CopyModulesAsync(List<SysModule> models);
    Task<Result> MoveModulesAsync(List<SysModule> models);
    Task<Result> MoveModuleAsync(SysModule model);
    Task<Result> SaveModuleAsync(SysModule model);
}

class ModuleService(Context context) : ServiceBase(context), IModuleService
{
    public Task<List<SysModule>> GetModulesAsync() => Database.QueryListAsync<SysModule>();

    public Task<SysModule> GetModuleAsync(string id) => Database.QueryByIdAsync<SysModule>(id);

    public async Task<Result> DeleteModulesAsync(List<SysModule> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        foreach (var model in models)
        {
            if (await Database.ExistsAsync<SysModule>(d => d.ParentId == model.Id))
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
            var module = await db.QueryAsync<SysModule>(d => d.ParentId == model.ParentId && d.Sort == sort);
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

    internal static async Task<List<SysModule>> GetModulesAsync(Database db)
    {
        var modules = await db.QueryListAsync<SysModule>(d => d.Enabled);
        if (db.User.IsTenantAdmin())
        {
            modules.RemoveModule("SysModuleList");
            modules.RemoveModule("SysTenantList");
        }
        return modules;
    }
}