namespace Known.Extensions;

/// <summary>
/// 后台数据扩展类。
/// </summary>
public static class CoreDataExtension
{
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
            return Result.Error(Language.TipTableRequired);

        try
        {
            try
            {
                var sql = $"select count(*) from {tableName}";
                var count = await db.ScalarAsync<int>(sql);
                if (count > 0)
                    return Result.Error(Language.TipTableHasData);

                sql = $"drop table {tableName}";
                await db.ExecuteAsync(sql);
            }
            catch
            {
            }

            await db.ExecuteAsync(script);
            return Result.Success(Language.ExecuteSuccess);
        }
        catch (Exception ex)
        {
            return Result.Error(ex.Message);
        }
    }

    internal static async Task<Database> GetDatabaseAsync(this Database database, AutoPageInfo info)
    {
        if (info == null || string.IsNullOrWhiteSpace(info.Database) || info.Database == database.ConnectionName)
            return database;

        if (Config.OnDatabase == null)
            return database;

        return await Config.OnDatabase.Invoke(database, info);
    }

    public static async Task<Result> InitializeTableAsync(this Database db)
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

    public static Task<Result> MigrateDataAsync(this Database db)
    {
        return MigrateHelper.MigrateDataAsync(db);
    }
}