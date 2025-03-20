namespace Known.Services;

partial class PlatformService
{
    public async Task<List<ModuleInfo>> GetModulesAsync()
    {
        var modules = await Database.QueryListAsync<SysModule>();
        var lists = modules.OrderBy(m => m.Sort).Select(m => m.ToModuleInfo()).ToList();
        DataHelper.Initialize(lists);
        return lists;
    }

    public async Task<Result> MigrateModulesAsync()
    {
        var topNav = await Database.GetConfigAsync(Constant.KeyTopNav);
        if (!string.IsNullOrWhiteSpace(topNav))
            return Result.Error("已经迁移过，无需再次迁移！");

        await Database.MigrateDataAsync();
        return Result.Success("迁移成功！");
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

    public async Task<Result> DeleteModulesAsync(List<ModuleInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var database = Database;
        foreach (var model in infos)
        {
            if (await database.ExistsAsync<SysModule>(d => d.ParentId == model.Id))
                return Result.Error(Language["Tip.ModuleDeleteExistsChild"]);
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

    public async Task<Result> InstallModulesAsync(List<ModuleInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync("安装", async db =>
        {
            var count = await db.CountAsync<SysModule>(d => d.ParentId == infos[0].ParentId);
            foreach (var item in infos)
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

    public async Task<Result> CopyModulesAsync(List<ModuleInfo> infos)
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

    public async Task<Result> MoveModulesAsync(List<ModuleInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

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

    public async Task<Result> MoveModuleAsync(ModuleInfo info)
    {
        if (info == null)
            return Result.Error(Language.SelectOne);

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

    public async Task<Result> SaveModuleAsync(ModuleInfo info)
    {
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