namespace Known.Repositories;

class FlowRepository
{
    internal static Task<List<SysFlow>> GetFlowsAsync(Database db, string bizIds)
    {
        if (string.IsNullOrWhiteSpace(bizIds))
            return null;

        var ids = bizIds.Replace(",", "','");
        var sql = $"select * from SysFlow where BizId in ('{ids}')";
        return db.QueryListAsync<SysFlow>(sql);
    }

    internal static Task<SysFlow> GetFlowAsync(Database db, string bizId)
    {
        var sql = "select * from SysFlow where BizId=@bizId";
        return db.QueryAsync<SysFlow>(sql, new { bizId });
    }

    internal static Task<UserInfo> GetFlowStepUserAsync(Database db, string appId, string compNo, string flowCode, string stepCode)
    {
        var sql = @"
select u.* from SysFlowStep s,SysUser u 
where s.OperateBy=u.UserName and s.AppId=@appId and s.CompNo=@compNo 
  and s.FlowCode=@flowCode and s.StepCode=@stepCode";
        return db.QueryAsync<UserInfo>(sql, new { appId, compNo, flowCode, stepCode });
    }

    internal static Task<int> DeleteFlowAsync(Database db, string bizId)
    {
        var sql = "delete from SysFlow where BizId=@bizId";
        return db.ExecuteAsync(sql, new { bizId });
    }

    internal static Task<int> DeleteFlowLogsAsync(Database db, string bizId)
    {
        var sql = "delete from SysFlowLog where BizId=@bizId";
        return db.ExecuteAsync(sql, new { bizId });
    }

    internal static Task<List<SysFlow>> GetFlowTodosAsync(Database db)
    {
        var sql = $"select * from SysFlow where FlowStatus='{FlowStatus.Open}' and BizStatus<>'{FlowStatus.Save}' and CurrBy=@UserName order by CreateTime";
        return db.QueryListAsync<SysFlow>(sql, new { db.User.UserName });
    }

    internal static Task<List<SysFlowLog>> GetFlowLogsAsync(Database db, string bizId)
    {
        var sql = "select * from SysFlowLog where BizId=@bizId order by ExecuteTime";
        return db.QueryListAsync<SysFlowLog>(sql, new { bizId });
    }
}