namespace Known.Core.Services;

public class LogService : BaseService
{
    internal LogService(Context context) : base(context) { }

    //Public
    public static List<string> GetVisitMenuIds(Database db, string userName, int size)
    {
        var logs = LogRepository.GetLogCounts(db, userName, Constants.LogTypePage)
                                .OrderByDescending(f => f.TotalCount).Take(size).ToList();
        return logs.Select(l => l.Field1).ToList();
    }

    public static void AddLog(Database db, string type, string target, string content)
    {
        db.Save(new SysLog
        {
            Type = type,
            Target = target,
            Content = content
        });
    }

    //Log
    internal PagingResult<SysLog> QueryLogs(PagingCriteria criteria)
    {
        return LogRepository.QueryLogs(Database, criteria);
    }

    internal Result DeleteLogs(string data)
    {
        var ids = Utils.FromJson<string[]>(data);
        var entities = Database.QueryListById<SysLog>(ids);
        if (entities == null || entities.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return Database.Transaction(Language.Delete, db =>
        {
            foreach (var item in entities)
            {
                db.Delete(item);
            }
        });
    }

    internal List<SysLog> GetLogs(string bizId) => LogRepository.GetLogs(Database, bizId);

    internal Result AddLog(SysLog log)
    {
        Database.Save(log);
        return Result.Success("添加成功！");
    }
}