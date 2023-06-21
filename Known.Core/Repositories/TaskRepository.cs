namespace Known.Core.Repositories;

class TaskRepository
{
    internal static SysTask GetPendingTaskByType(Database db, string type)
    {
        var sql = $"select * from SysTask where Type='{type}' and Status='{Constants.TaskPending}' order by CreateTime";
        return db.Query<SysTask>(sql);
    }

    internal static SysTask GetTaskByType(Database db, string type)
    {
        var sql = "select * from SysTask where CompNo=@CompNo and Type=@type order by CreateTime desc";
        return db.Query<SysTask>(sql, new { db.User.CompNo, type });
    }

    internal static SysTask GetTaskByBizId(Database db, string bizId)
    {
        var sql = "select * from SysTask where CompNo=@CompNo and CreateBy=@UserName and BizId=@bizId order by CreateTime desc";
        return db.Query<SysTask>(sql, new { db.User.CompNo, db.User.UserName, bizId });
    }
}