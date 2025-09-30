namespace Known.Services;

public interface IModule1Service : IService
{
    Task<List<SysModule1>> GetModulesAsync();
    Task<FileDataInfo> ExportModulesAsync();
    Task<Result> ImportModulesAsync(UploadInfo<FileFormInfo> info);
    Task<Result> DeleteModulesAsync(List<SysModule1> models);
    Task<Result> CopyModulesAsync(List<SysModule1> models);
    Task<Result> MoveModulesAsync(List<SysModule1> models);
    Task<Result> MoveModuleAsync(SysModule1 model);
    Task<Result> SaveModuleAsync(SysModule1 model);
}

[Client]
class Module1Client(HttpClient http) : ClientBase(http), IModule1Service
{
    public Task<List<SysModule1>> GetModulesAsync() => Http.GetAsync<List<SysModule1>>("/Module1/GetModules");
    public Task<FileDataInfo> ExportModulesAsync() => Http.GetAsync<FileDataInfo>("/Module1/ExportModules");
    public Task<Result> ImportModulesAsync(UploadInfo<FileFormInfo> info) => Http.PostAsync("/Module1/ImportModules", info);
    public Task<Result> DeleteModulesAsync(List<SysModule1> models) => Http.PostAsync("/Module1/DeleteModules", models);
    public Task<Result> CopyModulesAsync(List<SysModule1> models) => Http.PostAsync("/Module1/CopyModules", models);
    public Task<Result> MoveModulesAsync(List<SysModule1> models) => Http.PostAsync("/Module1/MoveModules", models);
    public Task<Result> MoveModuleAsync(SysModule1 model) => Http.PostAsync("/Module1/MoveModule", model);
    public Task<Result> SaveModuleAsync(SysModule1 model) => Http.PostAsync("/Module1/SaveModule", model);
}

[WebApi, Service]
class Module1Service(Context context) : ServiceBase(context), IModule1Service
{
    public async Task<List<SysModule1>> GetModulesAsync()
    {
        var modules = await Database.Query<SysModule1>().OrderBy(m => m.Sort).ToListAsync();
        var lists = modules.OrderBy(m => m.Sort).Select(m => m.ToModuleInfo()).ToList();
        DataHelper.Initialize(lists);
        return modules;
    }

    public async Task<FileDataInfo> ExportModulesAsync()
    {
        var modules = await Database.QueryListAsync<SysModule1>();
        var info = new FileDataInfo();
        info.Name = $"SysModule_{Config.App.Id}.kmd";
        info.Bytes = await ZipHelper.ZipDataAsync(modules);
        return info;
    }

    public async Task<Result> ImportModulesAsync(UploadInfo<FileFormInfo> info)
    {
        var key = nameof(FileFormInfo.BizType);
        if (info == null || info.Files == null || !info.Files.ContainsKey(key))
            return Result.Error(Language.ImportSelectFile);

        try
        {
            var file = info.Files[key][0];
            var modules = await ZipHelper.UnZipDataAsync<List<SysModule1>>(file.Bytes);
            if (modules != null && modules.Count > 0)
            {
                await Database.TransactionAsync(Language.Import, async db =>
                {
                    await db.DeleteAllAsync<SysModule1>();
                    await db.InsertListAsync(modules);
                });
            }
            return Result.Success(Language.Success(Language.Import));
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }

    public async Task<Result> DeleteModulesAsync(List<SysModule1> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var database = Database;
        foreach (var model in models)
        {
            if (await database.ExistsAsync<SysModule1>(d => d.ParentId == model.Id))
                return Result.Error(Language["Tip.ModuleDeleteExistsChild"]);
        }

        return await database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in models)
            {
                await db.DeleteAsync(item);
                await ResortModulesAsync(db, item.ParentId);
            }
        });
    }

    public async Task<Result> CopyModulesAsync(List<SysModule1> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Copy, async db =>
        {
            var count = await db.CountAsync<SysModule1>(d => d.ParentId == models[0].ParentId);
            foreach (var item in models)
            {
                item.Id = Utils.GetNextId();
                item.Sort = ++count;
                await db.InsertAsync(item);
            }
        });
    }

    public async Task<Result> MoveModulesAsync(List<SysModule1> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Save, async db =>
        {
            var count = await db.CountAsync<SysModule1>(d => d.ParentId == models[0].ParentId);
            foreach (var item in models)
            {
                item.Sort = ++count;
                await db.SaveAsync(item);
            }
        });
    }

    public async Task<Result> MoveModuleAsync(SysModule1 model)
    {
        if (model == null)
            return Result.Error(Language.SelectOne);

        return await Database.TransactionAsync(Language.Save, async db =>
        {
            var sort = model.IsMoveUp ? model.Sort - 1 : model.Sort + 1;
            var module = await db.QueryAsync<SysModule1>(d => d.ParentId == model.ParentId && d.Sort == sort);
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

    public async Task<Result> SaveModuleAsync(SysModule1 model)
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

    internal static async Task<List<SysModule1>> GetModulesAsync(Database db)
    {
        var modules = await db.QueryListAsync<SysModule1>(d => d.Enabled);
        if (db.User.IsTenantAdmin())
        {
            modules.RemoveModule("SysModuleList");
            modules.RemoveModule("SysTenantList");
        }
        return modules;
    }

    private static async Task ResortModulesAsync(Database db, string parentId)
    {
        var items = await db.Query<SysModule1>().Where(d => d.ParentId == parentId).OrderBy(d => d.Sort).ToListAsync();
        var index = 1;
        foreach (var item in items)
        {
            item.Sort = index++;
            await db.SaveAsync(item);
        }
    }
}