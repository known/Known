namespace Known.Helpers;

class MigrateHelper
{
    internal static async Task MigrateDataAsync(Database database)
    {
#if DEBUG
        try
        {
            if (Config.IsAdmin)
                return;

            database.EnableLog = false;
            var exists = await database.ExistsAsync<SysConfig>(true);
            if (!exists) //未安装，则安装时初始化
                return;

            database.User ??= await database.GetUserAsync(Constants.SysUserName);
            await database.TransactionAsync("迁移", async db =>
            {
                Console.WriteLine("AppData is Migrating...");
                await MigrateLanguagesAsync(db);
                await MigrateButtonsAsync(db);
                await MigrateTopNavsAsync(db);
                await MigrateModulesAsync(db);
                if (CoreConfig.OnMigrateAppData != null)
                    await CoreConfig.OnMigrateAppData.Invoke(db);
                Console.WriteLine("AppData is Migrated.");
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
#endif
    }

    private static async Task MigrateLanguagesAsync(Database db)
    {
        var datas = AppData.Data.Languages;
        if (datas == null || datas.Count == 0)
            return;

        if (await db.ExistsConfigAsync(Constant.KeyLanguage))
            return;

        await db.SaveConfigAsync(Constant.KeyLanguage, datas, true);
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

    private static async Task MigrateModulesAsync(Database db)
    {
        var table = DbConfig.Models.FirstOrDefault(m => m.Type == typeof(SysModule));
        if (table != null)
            await db.CreateTableAsync(table);

        var modules = await db.QueryListAsync<SysModule>();
        foreach (var module in modules)
        {
            if (string.IsNullOrWhiteSpace(module.Type))
            {
                module.Type = module.Target == nameof(ModuleType.Menu)
                            ? nameof(MenuType.Menu)
                            : (module.Target == nameof(ModuleType.Custom) ? nameof(MenuType.Link) : nameof(MenuType.Page));
                module.Target = module.Target == nameof(ModuleType.IFrame) ? nameof(LinkTarget.IFrame) : nameof(LinkTarget.None);
            }
            if (string.IsNullOrWhiteSpace(module.PluginData))
            {
                var plugins = module.ToPlugins();
                module.PluginData = plugins?.ZipDataString();
            }
        }

        var items = AppData.Data.Modules;
        if (items != null && items.Count > 0)
        {
            foreach (var item in items)
            {
                if (!modules.Exists(d => d.Name == item.Name))
                {
                    var module = SysModule.Load(db.User, item);
                    var parent = modules.FirstOrDefault(m => m.Code == item.ParentId);
                    module.ParentId = parent?.Id ?? "0";
                    modules.Add(module);
                }
            }
        }
        await db.DeleteAllAsync<SysModule>();
        await db.InsertListAsync(modules);
    }
}