namespace Known.Core.Repositories;

class FlowRepository
{
    internal static List<SysFlow> GetFlows(Database db, string bizIds)
    {
        if (string.IsNullOrWhiteSpace(bizIds))
            return null;

        var ids = bizIds.Replace(",", "','");
        var sql = $"select * from SysFlow where BizId in ('{ids}')";
        return db.QueryList<SysFlow>(sql);
    }

    internal static SysFlow GetFlow(Database db, string bizId)
    {
        var sql = "select * from SysFlow where BizId=@bizId";
        return db.Query<SysFlow>(sql, new { bizId });
    }

    internal static UserInfo GetFlowStepUser(Database db, string appId, string compNo, string flowCode, string stepCode)
    {
        var sql = @"
select u.* from SysFlowStep s,SysUser u 
where s.OperateBy=u.UserName and s.AppId=@appId and s.CompNo=@compNo 
  and s.FlowCode=@flowCode and s.StepCode=@stepCode";
        return db.Query<UserInfo>(sql, new { appId, compNo, flowCode, stepCode });
    }

    internal static void DeleteFlow(Database db, string bizId)
    {
        var sql = "delete from SysFlow where BizId=@bizId";
        db.Execute(sql, new { bizId });
    }

    internal static void DeleteFlowLogs(Database db, string bizId)
    {
        var sql = "delete from SysFlowLog where BizId=@bizId";
        db.Execute(sql, new { bizId });
    }

    internal static List<SysFlow> GetFlowTodos(Database db)
    {
        var sql = $"select * from SysFlow where FlowStatus='{FlowStatus.Open}' and AppId=@AppId and CurrBy=@UserName order by CreateTime";
        return db.QueryList<SysFlow>(sql, new { db.User.AppId, db.User.UserName });
    }

    internal static List<SysFlowLog> GetFlowLogs(Database db, string bizId)
    {
        var sql = "select * from SysFlowLog where BizId=@bizId order by ExecuteTime";
        return db.QueryList<SysFlowLog>(sql, new { bizId });
    }
}