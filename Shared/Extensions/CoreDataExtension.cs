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

    internal static async Task<Dictionary<string, List<TableSettingInfo>>> GetUserTableSettingsAsync(this Database db)
    {
        var settings = await db.GetUserSettingsAsync("UserTable_");
        if (settings == null || settings.Count == 0)
            return [];

        var dics = new Dictionary<string, List<TableSettingInfo>>();
        foreach (var item in settings)
        {
            dics[item.BizType] = item.DataAs<List<TableSettingInfo>>();
        }
        return dics;
    }

    internal static async Task SaveUserAsync(this Database db, InstallInfo info)
    {
        var userName = info.AdminName.ToLower();
        var user = await db.QueryAsync<SysUser>(d => d.UserName == userName);
        user ??= new SysUser();
        user.AppId = Config.App.Id;
        user.CompNo = info.CompNo;
        user.OrgNo = info.CompNo;
        user.UserName = userName;
        user.Password = Utils.ToMd5(info.AdminPassword);
        user.Name = info.AdminName;
        user.EnglishName = info.AdminName;
        user.Gender = "Male";
        user.Role = "Admin";
        user.Enabled = true;
        CoreConfig.OnNewUser?.Invoke(db, user);
        await db.SaveAsync(user);
    }
}