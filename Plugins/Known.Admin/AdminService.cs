namespace Known;

class AdminService : IAdminPService
{
    #region Task
    public Task<TaskInfo> GetTaskAsync(Database db, string bizId)
    {
        return db.Query<SysTask>().Where(d => d.CreateBy == db.UserName && d.BizId == bizId)
                 .OrderByDescending(d => d.CreateTime).FirstAsync<TaskInfo>();
    }

    public async Task CreateTaskAsync(Database db, TaskInfo info)
    {
        var task = new SysTask();
        if (!string.IsNullOrWhiteSpace(info.Id))
            task.Id = info.Id;
        task.BizId = info.BizId;
        task.Type = info.Type;
        task.Name = info.Name;
        task.Target = info.Target;
        task.Status = info.Status;
        await db.SaveAsync(task);
    }

    public async Task SaveTaskAsync(Database db, TaskInfo info)
    {
        var task = await db.QueryByIdAsync<SysTask>(info.Id);
        if (task == null)
            return;

        task.Status = info.Status;
        task.BeginTime = info.BeginTime;
        task.EndTime = info.EndTime;
        task.Note = info.Note;
        await db.SaveAsync(task);
    }
    #endregion

    #region Log
    public async Task<List<string>> GetVisitMenuIdsAsync(Database db, string userName, int size)
    {
        var logs = await db.Query<SysLog>()
                           .Where(d => d.Type == $"{LogType.Page}" && d.CreateBy == userName)
                           .GroupBy(d => d.Target)
                           .Select(d => new CountInfo { Field1 = d.Target, TotalCount = DbFunc.Count() })
                           .ToListAsync();
        logs = logs?.OrderByDescending(f => f.TotalCount).Take(size).ToList();
        return logs?.Select(l => l.Field1).ToList();
    }

    public Task SaveLogAsync(Database db, LogInfo info)
    {
        return db.SaveAsync(new SysLog
        {
            Type = info.Type.ToString(),
            Target = info.Target ?? "",
            Content = info.Content
        });
    }
    #endregion
}