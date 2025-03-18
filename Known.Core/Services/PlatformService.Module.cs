namespace Known.Services;

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