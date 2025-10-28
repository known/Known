namespace Known.Helpers;

class MigrateHelper
{
    internal static List<PluginInfo> TopNavs { get; set; } = [];

    internal static async Task<Result> MigrateDataAsync(Database database)
    {
        try
        {
            var exists = await database.ExistsAsync<SysConfig>(true);
            if (!exists) //未安装，则安装时初始化
                return Result.Error(Language.TipSystemNotInstall);

            database.User ??= await database.GetUserAsync(Constants.SysUserName);
            await database.CreateTablesAsync();
            return await database.TransactionAsync(Language.Migrate, async db =>
            {
                Console.WriteLine("AppData is Migrating...");
                await MigrateLanguagesAsync(db);
                await MigrateButtonsAsync(db);
                await MigrateTopNavsAsync(db);
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
        var items = Language.DefaultDatas;
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
        var buttons = Config.Actions.Select(d => d.ToButton()).ToList();
        if (buttons == null || buttons.Count == 0)
            return;

        var datas = new List<ButtonInfo>();
        datas.AddRange(buttons);
        var items = await db.GetConfigAsync<List<ButtonInfo>>(Constants.KeyButton, true);
        if (items != null && items.Count > 0)
        {
            foreach (var item in items)
            {
                if (!datas.Exists(d => d.Id == item.Id))
                    datas.Add(item);
            }
        }
        await db.SaveConfigAsync(Constants.KeyButton, datas, true);
    }

    private static async Task MigrateTopNavsAsync(Database db)
    {
        if (TopNavs == null || TopNavs.Count == 0)
            return;

        if (await db.ExistsConfigAsync(Constants.KeyTopNav))
            return;

        await db.SaveConfigAsync(Constants.KeyTopNav, TopNavs, true);
    }
}