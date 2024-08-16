namespace Known.SqlSugar;

class SqlSugarRepository : IDataRepository
{
    private readonly SqlSugarScope sugar;

    public SqlSugarRepository()
    {
        sugar = SqlSugarFactory.CreateSugar();
    }

    public async Task<PagingResult<SysUser>> QueryUsersAsync(Database db, PagingCriteria criteria)
    {
        var sql = sugar.Queryable<SysUser>()
                       .LeftJoin<SysOrganization>((u, o) => u.OrgNo == o.Id)
                       .Where(u => u.CompNo == db.User.CompNo && u.UserName != "admin")
                       .Select((u, o) => new SysUser { Id = o.Id.SelectAll(), Department = o.Name })
                       .ToSql();
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
        return await db.QueryPageAsync<SysUser>(sql.Key, criteria);
    }

    public Task<List<SysRole>> GetRolesAsync(Database db)
    {
        var sql = sugar.Queryable<SysRole>()
                       .Where(d => d.CompNo == db.User.CompNo && d.Enabled)
                       .OrderBy(d => d.CreateTime)
                       .ToSql();
        return sugar.QueryListAsync<SysRole>(sql.Key, sql.Value);
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
        var sql = sugar.Queryable<SysDictionary>()
                       .Where(d => d.CompNo == db.User.CompNo && d.Category == Constants.DicCategory && d.Enabled)
                       .OrderBy(d => d.Sort)
                       .ToSql();
        return sugar.QueryListAsync<SysDictionary>(sql.Key, sql.Value);
    }

    public Task<SysTask> GetPendingTaskAsync(Database db, string bizType)
    {
        var sql = sugar.Queryable<SysTask>()
                       .Where(d => d.CompNo == db.User.CompNo && d.Status == SysTaskStatus.Pending && d.Type == bizType)
                       .OrderBy(d => d.CreateTime)
                       .ToSql();
        return sugar.QueryAsync<SysTask>(sql.Key, sql.Value);
    }

    public Task<SysTask> GetTaskByTypeAsync(Database db, string type)
    {
        var sql = sugar.Queryable<SysTask>()
                       .Where(d => d.CompNo == db.User.CompNo && d.Type == type)
                       .OrderByDescending(d => d.CreateTime)
                       .ToSql();
        return sugar.QueryAsync<SysTask>(sql.Key, sql.Value);
    }

    public Task<SysTask> GetTaskByBizIdAsync(Database db, string bizId)
    {
        var sql = sugar.Queryable<SysTask>()
                       .Where(d => d.CompNo == db.User.CompNo && d.BizId == bizId)
                       .OrderByDescending(d => d.CreateTime)
                       .ToSql();
        return sugar.QueryAsync<SysTask>(sql.Key, sql.Value);
    }

    public Task<List<SysLog>> GetVisitLogsAsync(Database db, DateTime begin, DateTime end)
    {
        var sql = sugar.Queryable<SysLog>()
                       .Where(d => d.CompNo == db.User.CompNo && d.CreateTime >= begin && d.CreateTime <= end)
                       .ToSql();
        return sugar.QueryListAsync<SysLog>(sql.Key, sql.Value);
    }

    public Task<List<CountInfo>> GetVisitLogsAsync(Database db, string userName)
    {
        //var sql = $@"
        //select Target as Field1,count(*) as TotalCount 
        //from SysLog 
        //where Type='{LogType.Page}' and CreateBy=@userName 
        //group by Target";
        //return db.QueryListAsync<CountInfo>(sql, new { userName });
        var sql = sugar.Queryable<SysLog>()
                       .Where(d => d.Type == $"{LogType.Page}" && d.CreateBy == userName)
                       .GroupBy(d => d.Target)
                       .Select(d => new CountInfo { Field1 = d.Target, TotalCount = SqlFunc.AggregateCount(d.Id) })
                       .ToSql();
        return sugar.QueryListAsync<CountInfo>(sql.Key, sql.Value);
    }

    public Task<List<SysFlowLog>> GetFlowLogsAsync(Database db, string bizId)
    {
        var sql = sugar.Queryable<SysFlowLog>()
                       .Where(d => d.BizId == bizId)
                       .OrderBy(d => d.ExecuteTime)
                       .ToSql();
        return sugar.QueryListAsync<SysFlowLog>(sql.Key, sql.Value);
    }
}