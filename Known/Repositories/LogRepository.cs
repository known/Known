using Known.Entities;

namespace Known.Repositories;

class LogRepository
{
    internal static Task<PagingResult<SysLog>> QueryLogsAsync(Database db, PagingCriteria criteria)
    {
        var sql = "select * from SysLog where AppId=@AppId and CompNo=@CompNo";
        return db.QueryPageAsync<SysLog>(sql, criteria);
    }

    internal static Task<List<CountInfo>> GetLogCountsAsync(Database db, string userName, string logType)
    {
        var sql = "select Target as Field1,count(*) as TotalCount from SysLog where CreateBy=@userName and Type=@logType group by Target";
        return db.QueryListAsync<CountInfo>(sql, new { userName, logType });
    }

    internal static Task<List<SysLog>> GetLogsAsync(Database db, string bizId)
    {
        var sql = "select * from SysLog where Target=@bizId order by CreateTime";
        return db.QueryListAsync<SysLog>(sql, new { bizId });
    }
}