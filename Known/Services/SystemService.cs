namespace Known.Services;

public class SystemService : BaseService
{
    internal const string KeySystem = "SystemInfo";

    //Config
    public async Task<T> GetConfigAsync<T>(string key)
    {
        var json = await PlatformRepository.GetConfigAsync(Database, Config.AppId, key);
        return Utils.FromJson<T>(json);
    }

    public async Task<Result> SaveConfigAsync(ConfigInfo info)
    {
        await SaveConfigAsync(Database, info.Key, info.Value);
        return Result.Success("保存成功！");
    }

    //Task
    public Task<PagingResult<SysTask>> QueryTasksAsync(PagingCriteria criteria)
    {
        return TaskRepository.QueryTasksAsync(Database, criteria);
    }

    //Log
    public Task<PagingResult<SysLog>> QueryLogsAsync(PagingCriteria criteria)
    {
        return LogRepository.QueryLogsAsync(Database, criteria);
    }

    internal async Task<Result> DeleteLogsAsync(string data)
    {
        var ids = Utils.FromJson<string[]>(data);
        var entities = await Database.QueryListByIdAsync<SysLog>(ids);
        if (entities == null || entities.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in entities)
            {
                await db.DeleteAsync(item);
            }
        });
    }

    internal Task<List<SysLog>> GetLogsAsync(string bizId) => LogRepository.GetLogsAsync(Database, bizId);

    public async Task<Result> AddLogAsync(SysLog log)
    {
        await Database.SaveAsync(log);
        return Result.Success("添加成功！");
    }
}