namespace Known.Repositories;

class ModuleRepository
{
    internal static async Task<List<SysModule>> GetModulesAsync(Database db)
    {
        var modules = await db.QueryListAsync<SysModule>(d => d.Enabled);
        if (db.User.IsTenantAdmin())
        {
            modules.RemoveModule("SysModuleList");
            modules.RemoveModule("SysTenantList");
        }
        return modules;
    }

    internal static Task<SysModule> GetModuleAsync(Database db, string parentId, int sort)
    {
        if (string.IsNullOrEmpty(parentId))
        {
            var sql = "select * from SysModule where (ParentId='' or ParentId is null) and Sort=@sort";
            return db.QueryAsync<SysModule>(sql, new { parentId, sort });
        }

        return db.QueryAsync<SysModule>(d => d.ParentId == parentId && d.Sort == sort);
    }

    internal static Task<bool> ExistsChildAsync(Database db, string id)
    {
        var query = db.Query<SysModule>().Where(d => d.ParentId == id);
        if (db.User != null)
            query.Where(d => d.CompNo == db.User.CompNo);
        return query.ExistsAsync();
    }
}