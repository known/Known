namespace Known.Data;

/// <summary>
/// 系统框架数据依赖接口。
/// </summary>
public interface IDataRepository
{
    /// <summary>
    /// 异步查询用户分页数据。
    /// </summary>
    /// <param name="db">数据库访问对象。</param>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页查询结果。</returns>
    Task<PagingResult<SysUser>> QueryUsersAsync(Database db, PagingCriteria criteria);

    /// <summary>
    /// 异步获取系统角色列表。
    /// </summary>
    /// <param name="db">数据库访问对象。</param>
    /// <returns>系统角色列表。</returns>
    Task<List<SysRole>> GetRolesAsync(Database db);

    /// <summary>
    /// 异步获取用户角色模块ID列表。
    /// </summary>
    /// <param name="db">数据库访问对象。</param>
    /// <param name="userId">用户ID。</param>
    /// <returns>用户角色模块ID列表。</returns>
    Task<List<string>> GetRoleModuleIdsAsync(Database db, string userId);

    /// <summary>
    /// 异步获取数据字典类型列表。
    /// </summary>
    /// <param name="db">数据库访问对象。</param>
    /// <returns>数据字典类型列表。</returns>
    Task<List<SysDictionary>> GetDicCategoriesAsync(Database db);

    /// <summary>
    /// 异步获取第一个待执行的系统定时任务。
    /// </summary>
    /// <param name="db">数据库访问对象。</param>
    /// <param name="bizType">业务类型。</param>
    /// <returns>系统定时任务。</returns>
    Task<SysTask> GetPendingTaskAsync(Database db, string bizType);

    /// <summary>
    /// 异步获取系统定时任务。
    /// </summary>
    /// <param name="db">数据库访问对象。</param>
    /// <param name="type">任务类型。</param>
    /// <returns>系统定时任务。</returns>
    Task<SysTask> GetTaskByTypeAsync(Database db, string type);

    /// <summary>
    /// 异步获取系统定时任务。
    /// </summary>
    /// <param name="db">数据库访问对象。</param>
    /// <param name="bizId">业务数据ID。</param>
    /// <returns>系统定时任务。</returns>
    Task<SysTask> GetTaskByBizIdAsync(Database db, string bizId);

    /// <summary>
    /// 异步获取用户常用模块日志统计信息。
    /// </summary>
    /// <param name="db">数据库访问对象。</param>
    /// <param name="userName">用户名。</param>
    /// <returns>用户常用模块日志统计信息。</returns>
    Task<List<CountInfo>> GetVisitLogsAsync(Database db, string userName);

    /// <summary>
    /// 异步获取工作流日志列表。
    /// </summary>
    /// <param name="db">数据库访问对象。</param>
    /// <param name="bizId">业务数据ID。</param>
    /// <returns>工作流日志列表。</returns>
    Task<List<SysFlowLog>> GetFlowLogsAsync(Database db, string bizId);
}

class DataRepository : IDataRepository
{
    public async Task<PagingResult<SysUser>> QueryUsersAsync(Database db, PagingCriteria criteria)
    {
        var sql = $@"select a.*,b.{db.FormatName("Name")} as {db.FormatName("Department")} 
from {db.FormatName("SysUser")} a 
left join {db.FormatName("SysOrganization")} b on b.{db.FormatName("Id")}=a.{db.FormatName("OrgNo")} 
where a.{db.FormatName("CompNo")}=@CompNo and a.{db.FormatName("UserName")}<>'admin'";
        var orgNoId = nameof(SysUser.OrgNo);
        var orgNo = criteria.GetParameter<string>(orgNoId);
        if (!string.IsNullOrWhiteSpace(orgNo))
        {
            var org = await db.QueryByIdAsync<SysOrganization>(orgNo);
            if (org != null && org.Code != db.User.CompNo)
                criteria.SetQuery(orgNoId, QueryType.Equal, orgNo);
            else
                criteria.RemoveQuery(orgNoId);
        }
        criteria.Fields[nameof(SysUser.Name)] = $"a.{db.FormatName("Name")}";
        return await db.QueryPageAsync<SysUser>(sql, criteria);
        //return await db.Select<SysUser>()
        //               .LeftJoin<SysOrganization>((a, b) => b.Id == a.OrgNo)
        //               .Select<SysOrganization>(d => d.Name, "Department")
        //               .Where(d => d.CompNo == db.User.CompNo && d.UserName != "admin")
        //               .ToPageAsync(criteria);
    }

    public Task<List<SysRole>> GetRolesAsync(Database db)
    {
        //select * from SysRole where CompNo=@CompNo and Enabled='True' order by CreateTime
        var info = db.Query<SysRole>()
                     .Where(d => d.Enabled)
                     .OrderBy(d => d.CreateTime)
                     .ToCommand();
        return db.QueryListAsync<SysRole>(info);
    }

    public Task<List<string>> GetRoleModuleIdsAsync(Database db, string userId)
    {
        var sql = $@"select a.{db.FormatName("ModuleId")} from {db.FormatName("SysRoleModule")} a 
where a.{db.FormatName("RoleId")} in (select {db.FormatName("RoleId")} from {db.FormatName("SysUserRole")} where {db.FormatName("UserId")}=@UserId)
  and exists (select 1 from {db.FormatName("SysRole")} where {db.FormatName("Id")}=a.{db.FormatName("RoleId")} and {db.FormatName("Enabled")}='True')";
        return db.ScalarsAsync<string>(sql, new { UserId = userId });
    }

    public Task<List<SysDictionary>> GetDicCategoriesAsync(Database db)
    {
        //select * from SysDictionary where Enabled='True' and Category='{Constants.DicCategory}' and CompNo=@CompNo order by Sort
        var info = db.Query<SysDictionary>()
                     .Where(d => d.Enabled && d.Category == Constants.DicCategory)
                     .OrderBy(d => d.Sort)
                     .ToCommand();
        return db.QueryListAsync<SysDictionary>(info);
    }

    public Task<SysTask> GetPendingTaskAsync(Database db, string bizType)
    {
        //select * from SysTask where Status='{SysTaskStatus.Pending}' and Type=@bizType order by CreateTime
        var info = db.Query<SysTask>()
                     .Where(d => d.Status == SysTaskStatus.Pending && d.Type == bizType)
                     .OrderBy(d => d.CreateTime)
                     .ToCommand();
        return db.QueryAsync<SysTask>(info);
    }

    public Task<SysTask> GetTaskByTypeAsync(Database db, string type)
    {
        //select * from SysTask where CompNo=@CompNo and Type=@type order by CreateTime desc
        var info = db.Query<SysTask>()
                     .Where(d => d.Type == type)
                     .OrderByDescending(d => d.CreateTime)
                     .ToCommand();
        return db.QueryAsync<SysTask>(info);
    }

    public Task<SysTask> GetTaskByBizIdAsync(Database db, string bizId)
    {
        //select * from SysTask where CompNo=@CompNo and CreateBy=@UserName and BizId=@bizId order by CreateTime desc
        var info = db.Query<SysTask>()
                     .Where(d => d.CreateBy == db.UserName && d.BizId == bizId)
                     .OrderByDescending(d => d.CreateTime)
                     .ToCommand();
        return db.QueryAsync<SysTask>(info);
    }

    public Task<List<CountInfo>> GetVisitLogsAsync(Database db, string userName)
    {
        //select Target as Field1,count(*) as TotalCount 
        //from SysLog 
        //where Type='{LogType.Page}' and CreateBy=@userName 
        //group by Target
        var info = db.Query<SysLog>()
                     .Select(d => d.Target, nameof(CountInfo.Field1))
                     .SelectCount(nameof(CountInfo.TotalCount))
                     //.Select(d => new CountInfo { Field1 = d.Target, TotalCount = Sql.Count() })
                     .Where(d => d.Type == $"{LogType.Page}" && d.CreateBy == userName)
                     .GroupBy(d => d.Target)
                     .ToCommand();
        return db.QueryListAsync<CountInfo>(info);
    }

    public Task<List<SysFlowLog>> GetFlowLogsAsync(Database db, string bizId)
    {
        //select * from SysFlowLog where BizId=@bizId order by ExecuteTime
        var info = db.Query<SysFlowLog>()
                     .Where(d => d.BizId == bizId)
                     .OrderBy(d => d.CreateTime)
                     .ToCommand();
        return db.QueryListAsync<SysFlowLog>(info);
    }
}