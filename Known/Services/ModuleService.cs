namespace Known.Services;

/// <summary>
/// 模块服务接口。
/// </summary>
public interface IModuleService : IService
{
    /// <summary>
    /// 异步分页查询系统模块列表。
    /// </summary>
    /// <param name="criteria">查询条件。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<SysModule>> QueryModulesAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步获取系统模块列表。
    /// </summary>
    /// <returns>系统模块列表。</returns>
    Task<List<MenuInfo>> GetModulesAsync();

    /// <summary>
    /// 异步迁移老框架系统模块数据。
    /// </summary>
    /// <returns>迁移结果。</returns>
    Task<Result> MigrateModulesAsync();

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
    /// <param name="infos">系统模块列表。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteModulesAsync(List<SysModule> infos);

    /// <summary>
    /// 异步安装系统模块。
    /// </summary>
    /// <param name="infos">系统模块列表。</param>
    /// <returns>安装结果。</returns>
    Task<Result> InstallModulesAsync(List<SysModule> infos);

    /// <summary>
    /// 异步复制系统模块。
    /// </summary>
    /// <param name="infos">系统模块列表。</param>
    /// <returns>复制结果。</returns>
    Task<Result> CopyModulesAsync(List<SysModule> infos);

    /// <summary>
    /// 异步移动多条系统模块。
    /// </summary>
    /// <param name="infos">系统模块列表。</param>
    /// <returns>移动结果。</returns>
    Task<Result> MoveModulesAsync(List<SysModule> infos);

    /// <summary>
    /// 异步移动单条系统模块。
    /// </summary>
    /// <param name="info">系统模块信息。</param>
    /// <returns>移动结果。</returns>
    Task<Result> MoveModuleAsync(SysModule info);

    /// <summary>
    /// 异步保存系统模块。
    /// </summary>
    /// <param name="info">系统模块信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveModuleAsync(SysModule info);
}

[Client]
class ModuleClient(HttpClient http) : ClientBase(http), IModuleService
{
    public Task<PagingResult<SysModule>> QueryModulesAsync(PagingCriteria criteria) => Http.QueryAsync<SysModule>("/Module/QueryModules", criteria);
    public Task<List<MenuInfo>> GetModulesAsync() => Http.GetAsync<List<MenuInfo>>("/Module/GetModules");
    public Task<Result> MigrateModulesAsync() => Http.PostAsync("/Module/MigrateModules");
    public Task<FileDataInfo> ExportModulesAsync() => Http.GetAsync<FileDataInfo>("/Module/ExportModules");
    public Task<Result> ImportModulesAsync(UploadInfo<FileFormInfo> info) => Http.PostAsync("/Module/ImportModules", info);
    public Task<Result> DeleteModulesAsync(List<SysModule> infos) => Http.PostAsync("/Module/DeleteModules", infos);
    public Task<Result> InstallModulesAsync(List<SysModule> infos) => Http.PostAsync("/Module/InstallModules", infos);
    public Task<Result> CopyModulesAsync(List<SysModule> infos) => Http.PostAsync("/Module/CopyModules", infos);
    public Task<Result> MoveModulesAsync(List<SysModule> infos) => Http.PostAsync("/Module/MoveModules", infos);
    public Task<Result> MoveModuleAsync(SysModule info) => Http.PostAsync("/Module/MoveModule", info);
    public Task<Result> SaveModuleAsync(SysModule info) => Http.PostAsync("/Module/SaveModule", info);
}

[WebApi, Service]
class ModuleService(Context context) : ServiceBase(context), IModuleService
{
    public Task<PagingResult<SysModule>> QueryModulesAsync(PagingCriteria criteria)
    {
        //var modules = await Database.QueryListAsync<SysModule>();
        //var items = AppData.Data.Modules.Where(m => modules?.Exists(d => d.Url == m.Url) == false)
        //                                .OrderBy(d => d.ParentId)
        //                                .ThenBy(d => d.Sort)
        //                                .ToList();
        //var name = criteria.GetQueryValue(nameof(ModuleInfo.Name));
        //if (!string.IsNullOrEmpty(name))
        //    items = [.. items.Where(m => m.Name.Contains(name))];

        //return items.ToPagingResult(criteria);
        return Database.QueryPageAsync<SysModule>(criteria);
    }

    public async Task<List<MenuInfo>> GetModulesAsync()
    {
        var modules = await Database.QueryListAsync<SysModule>();
        var menus = modules.Select(m => m.ToMenuInfo()).ToList();
        //modules = modules.Add(AppData.Data.Modules);
        //DataHelper.Initialize(modules);
        return DataHelper.GetMenus(menus, false, true);
    }

    public async Task<Result> MigrateModulesAsync()
    {
        var topNav = await Database.GetConfigAsync(Constants.KeyTopNav);
        if (!string.IsNullOrWhiteSpace(topNav))
            return Result.Error(Language.TipModuleMigrated);

        return await Database.MigrateDataAsync();
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
            return Result.Error(Language.ImportSelectFile);

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
            return Result.Success(Language.ImportSuccess);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }

    public async Task<Result> DeleteModulesAsync(List<SysModule> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        if (infos.Any(d => d.IsCode))
            return Result.Error(Language.TipCodeModuleNotOperate);

        var database = Database;
        foreach (var model in infos)
        {
            if (await database.ExistsAsync<SysModule>(d => d.ParentId == model.Id))
                return Result.Error(Language.TipModuleDeleteExistsChild);
        }

        return await database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in infos)
            {
                await db.DeleteAsync<SysModule>(item.Id);
                await ResortModulesAsync(db, item.ParentId);
            }
        });
    }

    public async Task<Result> InstallModulesAsync(List<SysModule> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Install, async db =>
        {
            var count = await db.CountAsync<SysModule>(d => d.ParentId == infos[0].ParentId);
            foreach (var item in infos.OrderBy(d => d.Sort))
            {
                var model = new SysModule();
                model.FillModel(item);
                if (string.IsNullOrWhiteSpace(model.Code))
                    model.Code = model.Name;
                model.Sort = ++count;
                model.LayoutData = Utils.ToJson(item.Layout);
                model.PluginData = item.Plugins?.ZipDataString();
                await db.SaveAsync(model);
            }
        });
    }

    public async Task<Result> CopyModulesAsync(List<SysModule> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Copy, async db =>
        {
            var count = await db.CountAsync<SysModule>(d => d.ParentId == infos[0].ParentId);
            foreach (var item in infos)
            {
                var module = await db.QueryByIdAsync<SysModule>(item.Id);
                if (module != null)
                {
                    module.Id = Utils.GetNextId();
                    module.ParentId = item.ParentId;
                    module.Sort = ++count;
                    await db.InsertAsync(module);
                }
            }
        });
    }

    public async Task<Result> MoveModulesAsync(List<SysModule> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        if (infos.Any(d => d.IsCode))
            return Result.Error(Language.TipCodeModuleNotOperate);

        return await Database.TransactionAsync(Language.Save, async db =>
        {
            var count = await db.CountAsync<SysModule>(d => d.ParentId == infos[0].ParentId);
            foreach (var item in infos)
            {
                var module = await db.QueryByIdAsync<SysModule>(item.Id);
                if (module != null)
                {
                    module.ParentId = item.ParentId;
                    module.Sort = ++count;
                    await db.SaveAsync(module);
                }
            }
        });
    }

    public async Task<Result> MoveModuleAsync(SysModule info)
    {
        if (info == null)
            return Result.Error(Language.SelectOne);

        if (info.IsCode)
            return Result.Error(Language.TipCodeModuleNotOperate);

        return await Database.TransactionAsync(Language.Save, async db =>
        {
            var sort = info.IsMoveUp ? info.Sort - 1 : info.Sort + 1;
            var model = await db.QueryByIdAsync<SysModule>(info.Id);
            var module = await db.QueryAsync<SysModule>(d => d.ParentId == info.ParentId && d.Sort == sort);
            if (model != null && module != null)
            {
                module.Sort = info.Sort;
                await db.SaveAsync(module);

                if (info.IsMoveUp)
                    model.Sort--;
                else
                    model.Sort++;
                await db.SaveAsync(model);
            }
        });
    }

    public async Task<Result> SaveModuleAsync(SysModule info)
    {
        if (info.IsCode)
            return Result.Error(Language.TipCodeModuleNotOperate);

        var database = Database;
        var model = await database.QueryByIdAsync<SysModule>(info.Id);
        model ??= new SysModule();
        model.FillModel(info);

        var vr = model.Validate(Context);
        if (!vr.IsValid)
            return vr;

        if (string.IsNullOrWhiteSpace(model.Code))
            model.Code = model.Name;
        if (string.IsNullOrWhiteSpace(model.Icon))
            model.Icon = "";//AntDesign不识别null值

        model.LayoutData = Utils.ToJson(info.Layout);
        model.PluginData = info.Plugins?.ZipDataString();
        return await Database.TransactionAsync(Language.Save, async db =>
        {
            await db.SaveAsync(model);
            info.Id = model.Id;
        }, info);
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