﻿namespace Known.Helpers;

class MigrateHelper
{
    internal static async Task<Result> MigrateDataAsync(Database database)
    {
        try
        {
            if (Config.IsAdmin)
                return Result.Error(CoreLanguage.TipAdminNoMigrate);

            database.EnableLog = false;
            var exists = await database.ExistsAsync<SysConfig>(true);
            if (!exists) //未安装，则安装时初始化
                return Result.Error(CoreLanguage.TipSystemNotInstall);

            database.User ??= await database.GetUserAsync(Constants.SysUserName);
            await database.CreateTablesAsync();
            return await database.TransactionAsync(Language.Migrate, async db =>
            {
                Console.WriteLine("AppData is Migrating...");
                await MigrateLanguagesAsync(db);
                await MigrateButtonsAsync(db);
                await MigrateTopNavsAsync(db);
                //await MigrateModulesAsync(db);
                if (CoreConfig.OnMigrateAppData != null)
                    await CoreConfig.OnMigrateAppData.Invoke(db);
                Console.WriteLine("AppData is Migrated.");
            });
        }
        catch (Exception ex)
        {
            Logger.Exception(LogTarget.BackEnd, database.User, ex);
            return Result.Error(ex.Message);
        }
    }

    internal static async Task MigrateLanguagesAsync(Database db)
    {
        Language.Datas = await db.GetLanguagesAsync();
        var items = Language.GetDefaultLanguages();
        foreach (var item in items)
        {
            if (Language.Datas.Exists(m => m.Chinese == item.Chinese))
                continue;

            Language.Datas.Add(item);
            await db.SaveAsync(new SysLanguage { Chinese = item.Chinese });
        }
    }

    private static async Task MigrateButtonsAsync(Database db)
    {
        var buttons = AppData.Data.Buttons;
        if (buttons == null || buttons.Count == 0)
            return;

        var datas = new List<ButtonInfo>();
        datas.AddRange(buttons);
        var items = await db.GetConfigAsync<List<ButtonInfo>>(Constant.KeyButton, true);
        if (items != null && items.Count > 0)
        {
            foreach (var item in items)
            {
                if (!datas.Exists(d => d.Id == item.Id))
                    datas.Add(item);
            }
        }
        await db.SaveConfigAsync(Constant.KeyButton, datas, true);
    }

    private static async Task MigrateTopNavsAsync(Database db)
    {
        var datas = AppData.Data.TopNavs;
        if (datas == null || datas.Count == 0)
            return;

        if (await db.ExistsConfigAsync(Constant.KeyTopNav))
            return;

        await db.SaveConfigAsync(Constant.KeyTopNav, datas, true);
    }

    //private static async Task MigrateModulesAsync(Database db)
    //{
    //    var modules = await db.QueryListAsync<SysModule>();
    //    foreach (var module in modules)
    //    {
    //        if (string.IsNullOrWhiteSpace(module.Type))
    //        {
    //            module.Type = module.Target == nameof(ModuleType.Menu)
    //                        ? nameof(MenuType.Menu)
    //                        : (module.Target == nameof(ModuleType.Custom) ? nameof(MenuType.Link) : nameof(MenuType.Page));
    //            module.Target = module.Target == nameof(ModuleType.IFrame) ? nameof(LinkTarget.IFrame) : nameof(LinkTarget.None);
    //        }
    //        if (string.IsNullOrWhiteSpace(module.PluginData))
    //        {
    //            var plugins = module.ToPlugins();
    //            module.PluginData = plugins?.ZipDataString();
    //        }
    //    }

    //    var items = AppData.Data.Modules;
    //    if (items != null && items.Count > 0)
    //        AddModules(db, modules, items, "0");
    //    await db.DeleteAllAsync<SysModule>();
    //    await db.InsertAsync(modules);
    //}

    //private static void AddModules(Database db, List<SysModule> modules, List<ModuleInfo> allItems, string parentId)
    //{
    //    var items = allItems.Where(m => m.ParentId == parentId).OrderBy(m => m.Sort).ToList();
    //    if (items == null || items.Count == 0)
    //        return;

    //    foreach (var item in items)
    //    {
    //        if (!modules.Exists(m => m.Name == item.Name && m.Url == item.Url))
    //        {
    //            var module = SysModule.Load(db.User, item);
    //            var parent = modules.FirstOrDefault(m => m.Code == item.ParentId);
    //            module.ParentId = parent?.Id ?? "0";
    //            modules.Add(module);
    //            AddModules(db, modules, allItems, module.Code);
    //        }
    //    }
    //}
}