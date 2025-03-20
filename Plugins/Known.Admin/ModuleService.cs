﻿namespace Known;

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

[Client]
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

[WebApi, Service]
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
        var modules = await Database.QueryListAsync<SysModule>();
        var info = new FileDataInfo();
        info.Name = $"SysModule_{Config.App.Id}.kmd";
        info.Bytes = await ZipHelper.ZipDataAsync(modules);
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
            var modules = await ZipHelper.UnZipDataAsync<List<SysModule>>(file.Bytes);
            if (modules != null && modules.Count > 0)
            {
                await Database.TransactionAsync(Language.Import, async db =>
                {
                    await db.DeleteAllAsync<SysModule>();
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
                await ResortModulesAsync(db, item.ParentId);
            }
        });
    }

    public async Task<Result> CopyModulesAsync(List<SysModule> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Copy, async db =>
        {
            var count = await db.CountAsync<SysModule>(d => d.ParentId == models[0].ParentId);
            foreach (var item in models)
            {
                item.Id = Utils.GetNextId();
                item.Sort = ++count;
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
            var count = await db.CountAsync<SysModule>(d => d.ParentId == models[0].ParentId);
            foreach (var item in models)
            {
                item.Sort = ++count;
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

    private static async Task ResortModulesAsync(Database db, string parentId)
    {
        var items = await db.Query<SysModule>().Where(d => d.ParentId == parentId).OrderBy(d => d.Sort).ToListAsync();
        var index = 1;
        foreach (var item in items)
        {
            item.Sort = index++;
            await db.SaveAsync(item);
        }
    }
}