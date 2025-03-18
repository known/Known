namespace Known.Helpers;

class MigrateHelper
{
    internal static async Task MigrateDataAsync(Database database)
    {
#if DEBUG
        try
        {
            database.EnableLog = false;
            var exists = await database.ExistsAsync<SysConfig>();
            if (!exists) //未安装，则安装时初始化
                return;

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
        var datas = AppData.Data.Buttons;
        if (datas == null || datas.Count == 0)
            return;

        if (await db.ExistsConfigAsync(Constant.KeyButton))
            return;

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

    private static Task MigrateModulesAsync(Database db)
    {
        return Task.CompletedTask;
    }
}