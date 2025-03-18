namespace Known.Extensions;

/// <summary>
/// Admin数据扩展类。
/// </summary>
public static class DataExtension
{
    /// <summary>
    /// 异步获取常用功能菜单信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="userName">用户名。</param>
    /// <param name="size">Top数量。</param>
    /// <returns>功能菜单信息。</returns>
    public static async Task<List<string>> GetVisitMenuIdsAsync(this Database db, string userName, int size)
    {
        var logs = await db.Query<SysLog>()
                           .Where(d => d.Type == $"{LogType.Page}" && d.CreateBy == userName)
                           .GroupBy(d => d.Target)
                           .Select(d => new CountInfo { Field1 = d.Target, TotalCount = DbFunc.Count() })
                           .ToListAsync();
        logs = logs?.OrderByDescending(f => f.TotalCount).Take(size).ToList();
        return logs?.Select(l => l.Field1).ToList();
    }

    internal static async Task InitializeTableAsync(this Database db)
    {
        db.EnableLog = false;
        var exists = await db.ExistsAsync<SysConfig>();
        if (!exists)
        {
            Console.WriteLine("Table is initializing...");
            foreach (var item in DbConfig.Models)
            {
                await db.CreateTableAsync(item);
            }
            Console.WriteLine("Table is initialized.");
        }
    }

    internal static Task MigrateDataAsync(this Database db)
    {
        return MigrateHelper.MigrateDataAsync(db);
    }
}