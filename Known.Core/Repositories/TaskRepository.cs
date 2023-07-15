namespace Known.Core.Repositories;

class TaskRepository
{
    internal static PagingResult<SysTask> QueryTasks(Database db, PagingCriteria criteria)
    {
        var sql = "select * from SysTask where AppId=@AppId and CompNo=@CompNo";
        return db.QueryPage<SysTask>(sql, criteria);
    }

    internal static SysTask GetPendingTaskByType(Database db, string type)
    {
        var sql = $"select * from SysTask where Type='{type}' and Status='{TaskStatus.Pending}' order by CreateTime";
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