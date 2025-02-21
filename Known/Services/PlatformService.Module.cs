namespace Known.Services;

public partial interface IPlatformService
{
    /// <summary>
    /// 异步获取系统模块列表。
    /// </summary>
    /// <returns>系统模块列表。</returns>
    Task<List<ModuleInfo>> GetModulesAsync();

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
    public Task<List<ModuleInfo>> GetModulesAsync()
    {
        var infos = AppData.Data.Modules.OrderBy(m => m.Sort).ToList();
        return Task.FromResult(infos);
    }

    public async Task<FileDataInfo> ExportModulesAsync()
    {
        var modules = AppData.Data.Modules;
        var json = Utils.ToJson(modules);
        var info = new FileDataInfo();
        info.Name = $"SysModule_{Config.App.Id}.kmd";
        info.Bytes = await Utils.ZipDataAsync(json);
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
            var json = await Utils.UnZipDataAsync(file.Bytes);
            AppData.Data.Modules = Utils.FromJson<List<ModuleInfo>>(json);
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
        return Result.SuccessAsync(Language.DeleteSuccess);
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
        return Result.SuccessAsync(Language.SaveSuccess);
    }

    public Task<Result> SaveModuleAsync(ModuleInfo info)
    {
        if (string.IsNullOrWhiteSpace(info.Icon))
            info.Icon = "";//AntDesign不识别null值

        SaveModule(info);
        return Result.SuccessAsync(Language.SaveSuccess);
    }

    private void SaveModule(ModuleInfo info)
    {
        var module = AppData.Data.Modules.FirstOrDefault(d => d.Id == info.Id);
        if (module != null)
        {
            info.Id = module.Id;
            module = CreateModule(info);
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
            Plugins = info.Plugins,
        };
    }
}

partial class PlatformClient
{
    public Task<List<ModuleInfo>> GetModulesAsync()
    {
        return Http.GetAsync<List<ModuleInfo>>("/Platform/GetModules");
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