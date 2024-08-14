namespace Known.Data;

public interface IDataRepository
{
    Task<PagingResult<SysUser>> QueryUsersAsync(Database db, PagingCriteria criteria);
    Task<List<SysRole>> GetRolesAsync(Database db);
    Task<List<string>> GetRoleModuleIdsAsync(Database db, string userId);
    Task<List<SysDictionary>> GetDicCategoriesAsync(Database db);
    Task<SysTask> GetPendingTaskAsync(Database db, string bizType);
    Task<SysTask> GetTaskByTypeAsync(Database db, string type);
    Task<SysTask> GetTaskByBizIdAsync(Database db, string bizId);
    Task<List<SysLog>> GetVisitLogsAsync(Database db, DateTime begin, DateTime end);
    Task<List<CountInfo>> GetVisitLogsAsync(Database db, string userName);
    Task<List<SysFlowLog>> GetFlowLogsAsync(Database db, string bizId);
}

class DataRepository : IDataRepository
{
    public async Task<PagingResult<SysUser>> QueryUsersAsync(Database db, PagingCriteria criteria)
    {
        var sql = @"
select a.*,b.Name as Department 
from SysUser a 
left join SysOrganization b on b.Id=a.OrgNo 
where a.CompNo=@CompNo and a.UserName<>'admin'";
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
        criteria.Fields[nameof(SysUser.Name)] = "a.Name";
        return await db.QueryPageAsync<SysUser>(sql, criteria);
        //return await db.Select<SysUser>()
        //               .LeftJoin<SysOrganization>((a, b) => b.Id == a.OrgNo)
        //               .Select<SysOrganization>(d => d.Name, "Department")
        //               .Where(d => d.CompNo == db.User.CompNo && d.UserName != "admin")
        //               .ToPageAsync(criteria);
    }

    public Task<List<SysRole>> GetRolesAsync(Database db)
    {
        var sql = "select * from SysRole where CompNo=@CompNo and Enable='True' order by CreateTime";
        return db.QueryListAsync<SysRole>(sql, new { db.User.CompNo });
    }

    public Task<List<string>> GetRoleModuleIdsAsync(Database db, string userId)
    {
        var sql = @"select a.ModuleId from SysRoleModule a 
where a.RoleId in (select RoleId from SysUserRole where UserId=@UserId)
  and exists (select 1 from SysRole where Id=a.RoleId and Enabled='True')";
        return db.ScalarsAsync<string>(sql, new { UserId = userId });
    }

    public Task<List<SysDictionary>> GetDicCategoriesAsync(Database db)
    {
        var sql = $"select * from SysDictionary where Enabled='True' and Category='{Constants.DicCategory}' and CompNo=@CompNo order by Sort";
        return db.QueryListAsync<SysDictionary>(sql, new { db.User.CompNo });
    }

    public Task<SysTask> GetPendingTaskAsync(Database db, string bizType)
    {
        var sql = $"select * SysTask where Status='{SysTaskStatus.Pending}' and Type=@bizType order by CreateTime";
        return db.QueryAsync<SysTask>(sql, new { bizType });
    }

    public Task<SysTask> GetTaskByTypeAsync(Database db, string type)
    {
        var sql = "select * from SysTask where CompNo=@CompNo and Type=@type order by CreateTime desc";
        return db.QueryAsync<SysTask>(sql, new { db.User.CompNo, type });
    }

    public Task<SysTask> GetTaskByBizIdAsync(Database db, string bizId)
    {
        var sql = "select * from SysTask where CompNo=@CompNo and CreateBy=@UserName and BizId=@bizId order by CreateTime desc";
        return db.QueryAsync<SysTask>(sql, new { db.User.CompNo, db.User.UserName, bizId });
    }

    public Task<List<SysLog>> GetVisitLogsAsync(Database db, DateTime begin, DateTime end)
    {
        var arg1 = begin.ToString("yyyy-MM-dd HH:mm:ss");
        var arg2 = end.ToString("yyyy-MM-dd HH:mm:ss");
        var sql = $"select * from SysLog where CompNo=@CompNo and CreateTime between '{arg1}' and '{arg2}'";
        return db.QueryListAsync<SysLog>(sql, new { db.User.CompNo });
    }

    public Task<List<CountInfo>> GetVisitLogsAsync(Database db, string userName)
    {
        var sql = $@"
select Target as Field1,count(*) as TotalCount 
from SysLog 
where Type='{LogType.Page}' and CreateBy=@userName 
group by Target";
        return db.QueryListAsync<CountInfo>(sql, new { userName });
    }

    public Task<List<SysFlowLog>> GetFlowLogsAsync(Database db, string bizId)
    {
        var sql = "select * from SysFlowLog where BizId=@bizId order by ExecuteTime";
        return db.QueryListAsync<SysFlowLog>(sql, new { bizId });
    }
}