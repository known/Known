namespace Known.Services;

/// <summary>
/// 系统模块服务接口。
/// </summary>
public interface IModuleService : IService
{
    /// <summary>
    /// 异步获取系统模块列表。
    /// </summary>
    /// <returns>系统模块列表。</returns>
    Task<List<SysModule>> GetModulesAsync();

    /// <summary>
    /// 异步导出系统模块数据。
    /// </summary>
    /// <returns>导出文件对象。</returns>
    Task<FileDataInfo> ExportModulesAsync();

    /// <summary>
    /// 异步导入系统模块数据。
    /// </summary>
    /// <param name="info">导入文件。</param>
    /// <returns>导入结果。</returns>
    Task<Result> ImportModulesAsync(UploadInfo<FileFormInfo> info);

    /// <summary>
    /// 异步删除系统模块。
    /// </summary>
    /// <param name="models">系统模块列表。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteModulesAsync(List<SysModule> models);

    /// <summary>
    /// 异步复制系统模块。
    /// </summary>
    /// <param name="models">系统模块列表。</param>
    /// <returns>复制结果。</returns>
    Task<Result> CopyModulesAsync(List<SysModule> models);

    /// <summary>
    /// 异步移动多条系统模块。
    /// </summary>
    /// <param name="models">系统模块列表。</param>
    /// <returns>移动结果。</returns>
    Task<Result> MoveModulesAsync(List<SysModule> models);

    /// <summary>
    /// 异步移动单条系统模块。
    /// </summary>
    /// <param name="model">系统模块信息。</param>
    /// <returns>移动结果。</returns>
    Task<Result> MoveModuleAsync(SysModule model);

    /// <summary>
    /// 异步保存系统模块。
    /// </summary>
    /// <param name="model">系统模块信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveModuleAsync(SysModule model);
}

class ModuleClient(HttpClient http) : ClientBase(http), IModuleService
{
    public Task<List<SysModule>> GetModulesAsync()
    {
        return Http.GetAsync<List<SysModule>>("/Module/GetModules");
    }

    public Task<FileDataInfo> ExportModulesAsync()
    {
        return Http.GetAsync<FileDataInfo>("/Module/ExportModules");
    }

    public Task<Result> ImportModulesAsync(UploadInfo<FileFormInfo> info)
    {
        return Http.PostAsync("/Module/ImportModules", info);
    }

    public Task<Result> DeleteModulesAsync(List<SysModule> models)
    {
        return Http.PostAsync("/Module/DeleteModules", models);
    }

    public Task<Result> CopyModulesAsync(List<SysModule> models)
    {
        return Http.PostAsync("/Module/CopyModules", models);
    }

    public Task<Result> MoveModulesAsync(List<SysModule> models)
    {
        return Http.PostAsync("/Module/MoveModules", models);
    }

    public Task<Result> MoveModuleAsync(SysModule model)
    {
        return Http.PostAsync("/Module/MoveModule", model);
    }

    public Task<Result> SaveModuleAsync(SysModule model)
    {
        return Http.PostAsync("/Module/SaveModule", model);
    }
}

[WebApi]
class ModuleService(Context context) : ServiceBase(context), IModuleService
{
    public async Task<List<SysModule>> GetModulesAsync()
    {
        var modules = await Database.Query<SysModule>().OrderBy(m => m.Sort).ToListAsync();
        var lists = modules.OrderBy(m => m.Sort).Select(m => m.ToModuleInfo()).ToList();
        DataHelper.Initialize(lists);
        return modules;
    }

    public async Task<FileDataInfo> ExportModulesAsync()
    {
        var info = new FileDataInfo();
        info.Name = $"SysModule_{Config.App.Id}.kmd";
        info.Bytes = await ModuleHelper.ExportModulesAsync(Database);
        return info;
    }

    public async Task<Result> ImportModulesAsync(UploadInfo<FileFormInfo> info)
    {
        var key = nameof(FileFormInfo.BizType);
        if (info == null || info.Files == null || !info.Files.ContainsKey(key))
            return Result.Error(Language["Import.SelectFile"]);

        try
        {
            var file = info.Files[key][0];
            await ModuleHelper.ImportModulesAsync(Database, file);
            return Result.Success(Language.Success(Language.Import));
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }

    public async Task<Result> DeleteModulesAsync(List<SysModule> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var database = Database;
        foreach (var model in models)
        {
            if (await database.ExistsAsync<SysModule>(d => d.ParentId == model.Id))
                return Result.Error(Language["Tip.ModuleDeleteExistsChild"]);
        }

        return await database.TransactionAsync(Language.Delete, async db =>
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