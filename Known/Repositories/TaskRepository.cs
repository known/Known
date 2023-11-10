using Known.Entities;

namespace Known.Repositories;

class TaskRepository
{
    internal static Task<PagingResult<SysTask>> QueryTasksAsync(Database db, PagingCriteria criteria)
    {
        var sql = "select * from SysTask where AppId=@AppId and CompNo=@CompNo";
        return db.QueryPageAsync<SysTask>(sql, criteria);
    }

    internal static Task<SysTask> GetPendingTaskByTypeAsync(Database db, string type)
    {
        var sql = $"select * from SysTask where Type='{type}' and Status='{TaskStatus.Pending}' order by CreateTime";
        return db.QueryAsync<SysTask>(sql);
    }

    internal static Task<SysTask> GetTaskByTypeAsync(Database db, string type)
    {
        var sql = "select * from SysTask where CompNo=@CompNo and Type=@type order by CreateTime desc";
        return db.QueryAsync<SysTask>(sql, new { db.User.CompNo, type });
    }

    internal static Task<SysTask> GetTaskByBizIdAsync(Database db, string bizId)
    {
        var sql = "select * from SysTask where CompNo=@CompNo and CreateBy=@UserName and BizId=@bizId order by CreateTime desc";
        return db.QueryAsync<SysTask>(sql, new { db.User.CompNo, db.User.UserName, bizId });
    }
}