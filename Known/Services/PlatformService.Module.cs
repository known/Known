namespace Known.Services;

public partial interface IPlatformService
{
    /// <summary>
    /// 异步分页查询系统模块列表。
    /// </summary>
    /// <param name="criteria">查询条件。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<ModuleInfo>> QueryModulesAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步获取系统模块列表。
    /// </summary>
    /// <returns>系统模块列表。</returns>
    Task<List<ModuleInfo>> GetModulesAsync();

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
    Task<Result> DeleteModulesAsync(List<ModuleInfo> infos);

    /// <summary>
    /// 异步安装系统模块。
    /// </summary>
    /// <param name="infos">系统模块列表。</param>
    /// <returns>安装结果。</returns>
    Task<Result> InstallModulesAsync(List<ModuleInfo> infos);

    /// <summary>
    /// 异步复制系统模块。
    /// </summary>
    /// <param name="infos">系统模块列表。</param>
    /// <returns>复制结果。</returns>
    Task<Result> CopyModulesAsync(List<ModuleInfo> infos);

    /// <summary>
    /// 异步移动多条系统模块。
    /// </summary>
    /// <param name="infos">系统模块列表。</param>
    /// <returns>移动结果。</returns>
    Task<Result> MoveModulesAsync(List<ModuleInfo> infos);

    /// <summary>
    /// 异步移动单条系统模块。
    /// </summary>
    /// <param name="info">系统模块信息。</param>
    /// <returns>移动结果。</returns>
    Task<Result> MoveModuleAsync(ModuleInfo info);

    /// <summary>
    /// 异步保存系统模块。
    /// </summary>
    /// <param name="info">系统模块信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveModuleAsync(ModuleInfo info);
}

partial class PlatformService
{
    public Task<PagingResult<ModuleInfo>> QueryModulesAsync(PagingCriteria criteria)
    {
        var modules = AppData.Data.Modules.OrderBy(d => d.ParentId)
                                          .ThenBy(d => d.Sort)
                                          .ToList();
        var name = criteria.GetQueryValue(nameof(ModuleInfo.Name));
        if (!string.IsNullOrEmpty(name))
            modules = [.. modules.Where(m => m.Name.Contains(name))];
        var result = modules.ToPagingResult(criteria);
        return Task.FromResult(result);
    }

    public Task<List<ModuleInfo>> GetModulesAsync()
    {
        var infos = AppData.Data.Modules.OrderBy(m => m.Sort).ToList();
        return Task.FromResult(infos);
    }

    public Task<Result> MigrateModulesAsync()
    {
        return Result.SuccessAsync("迁移成功！");
    }

    public async Task<FileDataInfo> ExportModulesAsync()
    {
        var modules = AppData.Data.Modules;
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
            AppData.Data.Modules = await ZipHelper.UnZipDataAsync<List<ModuleInfo>>(file.Bytes);
            AppData.SaveData();
            return Result.Success(Language.Success(Language.Import));
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }

    public Task<Result> DeleteModulesAsync(List<ModuleInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.ErrorAsync(Language.SelectOneAtLeast);

        foreach (var item in infos)
        {
            if (AppData.Data.Modules.Any(d => d.ParentId == item.Id))
                return Result.ErrorAsync(Language["Tip.ModuleDeleteExistsChild"]);

            var module = AppData.Data.Modules.FirstOrDefault(d => d.Id == item.Id);
            if (module != null)
                AppData.Data.Modules.Remove(module);
        }
        AppData.SaveData();
        return Result.SuccessAsync(Language.DeleteSuccess);
    }

    public Task<Result> InstallModulesAsync(List<ModuleInfo> infos)
    {
        return Result.SuccessAsync("安装成功！");
    }

    public Task<Result> CopyModulesAsync(List<ModuleInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.ErrorAsync(Language.SelectOneAtLeast);

        foreach (var item in infos)
        {
            var module = AppData.Data.Modules.FirstOrDefault(d => d.Id == item.Id);
            if (module != null)
            {
                var newModule = CreateModule(module);
                newModule.Id = Utils.GetNextId();
                newModule.ParentId = item.ParentId;
                AppData.Data.Modules.Add(newModule);
            }
        }
        AppData.SaveData();
        return Result.SuccessAsync(Language.Success(Language.Copy));
    }

    public Task<Result> MoveModulesAsync(List<ModuleInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.ErrorAsync(Language.SelectOneAtLeast);

        foreach (var item in infos)
        {
            SaveModule(item);
        }
        AppData.SaveData();
        return Result.SuccessAsync(Language.SaveSuccess);
    }

    public Task<Result> MoveModuleAsync(ModuleInfo info)
    {
        if (info == null)
            return Result.ErrorAsync(Language.SelectOne);

        var sort = info.IsMoveUp ? info.Sort - 1 : info.Sort + 1;
        var module = AppData.Data.Modules.FirstOrDefault(d => d.ParentId == info.ParentId && d.Sort == sort);
        if (module != null)
        {
            module.Sort = info.Sort;

            if (info.IsMoveUp)
                info.Sort--;
            else
                info.Sort++;
        }
        AppData.SaveData();
        return Result.SuccessAsync(Language.SaveSuccess);
    }

    public Task<Result> SaveModuleAsync(ModuleInfo info)
    {
        if (string.IsNullOrWhiteSpace(info.Icon))
            info.Icon = "";//AntDesign不识别null值

        SaveModule(info);
        AppData.SaveData();
        return Result.SuccessAsync(Language.SaveSuccess);
    }

    private static void SaveModule(ModuleInfo info)
    {
        var module = AppData.Data.Modules.FirstOrDefault(d => d.Id == info.Id);
        if (module != null)
        {
            module.ParentId = info.ParentId;
            module.Name = info.Name;
            module.Icon = info.Icon;
            module.Type = info.Type;
            module.Target = info.Target;
            module.Url = info.Url;
            module.Sort = info.Sort;
            module.Enabled = info.Enabled;
            module.Layout = info.Layout;
            module.Plugins = info.Plugins;
        }
        else
        {
            AppData.Data.Modules.Add(info);
        }
    }

    private static ModuleInfo CreateModule(ModuleInfo info)
    {
        return new ModuleInfo
        {
            Id = info.Id,
            ParentId = info.ParentId,
            Name = info.Name,
            Icon = info.Icon,
            Type = info.Type,
            Target = info.Target,
            Url = info.Url,
            Sort = info.Sort,
            Enabled = info.Enabled,
            Layout = info.Layout,
            Plugins = info.Plugins
        };
    }
}

partial class PlatformClient
{
    public Task<PagingResult<ModuleInfo>> QueryModulesAsync(PagingCriteria criteria)
    {
        return Http.QueryAsync<ModuleInfo>("/Platform/QueryModules", criteria);
    }

    public Task<List<ModuleInfo>> GetModulesAsync()
    {
        return Http.GetAsync<List<ModuleInfo>>("/Platform/GetModules");
    }

    public Task<Result> MigrateModulesAsync()
    {
        return Http.PostAsync("/Platform/MigrateModules");
    }

    public Task<FileDataInfo> ExportModulesAsync()
    {
        return Http.GetAsync<FileDataInfo>("/Platform/ExportModules");
    }

    public Task<Result> ImportModulesAsync(UploadInfo<FileFormInfo> info)
    {
        return Http.PostAsync("/Platform/ImportModules", info);
    }

    public Task<Result> DeleteModulesAsync(List<ModuleInfo> infos)
    {
        return Http.PostAsync("/Platform/DeleteModules", infos);
    }

    public Task<Result> InstallModulesAsync(List<ModuleInfo> infos)
    {
        return Http.PostAsync("/Platform/InstallModules", infos);
    }

    public Task<Result> CopyModulesAsync(List<ModuleInfo> infos)
    {
        return Http.PostAsync("/Platform/CopyModules", infos);
    }

    public Task<Result> MoveModulesAsync(List<ModuleInfo> infos)
    {
        return Http.PostAsync("/Platform/MoveModules", infos);
    }

    public Task<Result> MoveModuleAsync(ModuleInfo info)
    {
        return Http.PostAsync("/Platform/MoveModule", info);
    }

    public Task<Result> SaveModuleAsync(ModuleInfo info)
    {
        return Http.PostAsync("/Platform/SaveModule", info);
    }
}