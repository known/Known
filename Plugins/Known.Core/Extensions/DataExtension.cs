namespace Known.Extensions;

/// <summary>
/// Admin数据扩展类。
/// </summary>
public static class DataExtension
{
    /// <summary>
    /// 根据ID获取实体插件参数配置信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="moduleId">模块ID。</param>
    /// <param name="pluginId">插件ID。</param>
    /// <returns>实体插件参数配置信息。</returns>
    public static async Task<AutoPageInfo> GetAutoPageAsync(this Database db, string moduleId, string pluginId)
    {
        var entity = await db.QueryByIdAsync<SysModule>(moduleId);
        if (entity?.Target == nameof(ModuleType.Page)) // Admin插件无代码配置
            return entity?.ToAutoPageInfo();

        var module = entity?.ToModuleInfo();
        if (module != null && module.Plugins != null && module.Plugins.Count > 0)
            return module.Plugins.GetPluginParameter<AutoPageInfo>(pluginId);

        var plugins = new List<PluginInfo>();
        foreach (var item in AppData.Data.Modules)
        {
            if (item.Plugins != null && item.Plugins.Count > 0)
                plugins.AddRange(item.Plugins);
        }

        var plugin = plugins.FirstOrDefault(p => p.Id == pluginId);
        if (plugin == null)
            return null;

        if (AppData.OnAutoPage != null)
            return AppData.OnAutoPage.Invoke(plugin);

        return Utils.FromJson<AutoPageInfo>(plugin.Setting);
    }

    /// <summary>
    /// 异步创建数据表。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="tableName">数据表名。</param>
    /// <param name="script">建表脚本。</param>
    /// <returns></returns>
    public static async Task<Result> CreateTableAsync(this Database db, string tableName, string script)
    {
        if (string.IsNullOrWhiteSpace(tableName))
            return Result.Error(CoreLanguage.TipTableRequired);

        try
        {
            try
            {
                var sql = $"select count(*) from {tableName}";
                var count = await db.ScalarAsync<int>(sql);
                if (count > 0)
                    return Result.Error(CoreLanguage.TipTableHasData);

                sql = $"drop table {tableName}";
                await db.ExecuteAsync(sql);
            }
            catch
            {
            }

            await db.ExecuteAsync(script);
            return Result.Success(CoreLanguage.ExecuteSuccess);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }

    internal static async Task<Result> InitializeTableAsync(this Database db)
    {
        try
        {
            var exists = await db.ExistsAsync<SysConfig>();
            if (!exists)
            {
                Console.WriteLine("Table is initializing...");
                await db.CreateTablesAsync();
                Console.WriteLine("Table is initialized.");
            }
            return Result.Success("Initialize successful!");
        }
        catch (Exception ex)
        {
            Logger.Exception(ex);
            return Result.Error(ex.Message);
        }
    }

    internal static Task<Result> MigrateDataAsync(this Database db)
    {
        return MigrateHelper.MigrateDataAsync(db);
    }

    internal static async Task<Database> GetDatabaseAsync(this Database database, AutoPageInfo info)
    {
        if (info == null || string.IsNullOrWhiteSpace(info.Database) || info.Database == database.ConnectionName)
            return database;

        if (CoreConfig.OnDatabase == null)
            return database;

        return await CoreConfig.OnDatabase.Invoke(database, info);
    }
}