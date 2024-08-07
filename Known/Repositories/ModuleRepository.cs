﻿namespace Known.Repositories;

class ModuleRepository
{
    internal static async Task<List<SysModule>> GetModulesAsync(Database db)
    {
        var sql = "select * from SysModule where Enabled='True'";
        var modules = await db.QueryListAsync<SysModule>(sql);
        if (db.User.IsTenantAdmin())
        {
            modules.RemoveModule("SysModuleList");
            modules.RemoveModule("SysTenantList");
        }
        return modules;
    }

    internal static Task<SysModule> GetModuleAsync(Database db, string parentId, int sort)
    {
        var sql = string.IsNullOrWhiteSpace(parentId)
                ? "select * from SysModule where (ParentId='' or ParentId is null) and Sort=@sort"
                : "select * from SysModule where ParentId=@parentId and Sort=@sort";
        return db.QueryAsync<SysModule>(sql, new { parentId, sort });
    }

    internal static Task<SysModule> GetModuleByUrlAsync(Database db, string url)
    {
        var sql = "select * from SysModule where Url=@url";
        return db.QueryAsync<SysModule>(sql, new { url });
    }

    internal static async Task<bool> ExistsChildAsync(Database db, string id)
    {
        var sql = "select count(*) from SysModule where ParentId=@id";
        if (db.User != null)
            sql += $" and CompNo='{db.User.CompNo}'";
        return await db.ScalarAsync<int>(sql, new { id }) > 0;
    }
}