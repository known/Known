namespace Known.Core.Repositories;

class ModuleRepository
{
    internal static List<SysModule> GetModules(Database db)
    {
        var sql = "select * from SysModule where Enabled='True'";
        return db.QueryList<SysModule>(sql);
    }

    internal static SysModule GetModule(Database db, string parentId, int sort)
    {
        var sql = string.IsNullOrWhiteSpace(parentId)
                ? "select * from SysModule where (ParentId='' or ParentId is null) and Sort=@sort"
                : "select * from SysModule where ParentId=@parentId and Sort=@sort";
        return db.Query<SysModule>(sql, new { parentId, sort });
    }

    internal static bool ExistsChild(Database db, string id)
    {
        var sql = "select count(*) from SysModule where ParentId=@id";
        return db.Scalar<int>(sql, new { id }) > 0;
    }
}