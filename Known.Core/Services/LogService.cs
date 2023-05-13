namespace Known.Core.Services;

public class LogService : BaseService
{
    internal LogService(Context context) : base(context) { }

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

    public static List<string> GetVisitMenuIds(Database db, string userId, int size)
    {
        var logs = LogRepository.GetLogCounts(db, userId, Constants.LogTypePage)
                                .OrderByDescending(f => f.TotalCount).Take(size).ToList();
        return logs.Select(l => l.Field1).ToList();
    }

    internal List<SysLog> GetLogs(string bizId) => LogRepository.GetLogs(Database, bizId);

    internal Result AddLog(SysLog log)
    {
        //if (!Config.IsDevelopment)
        Database.Save(log);
        return Result.Success("添加成功！");
    }

    internal static void AddLog(Database db, string type, string target, string content)
    {
        //if (Config.IsDevelopment)
        //    return;

        db.Save(new SysLog
        {
            Type = type,
            Target = target,
            Content = content
        });
    }
}