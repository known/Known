namespace Known.Data;

public partial class Database
{
    /// <summary>
    /// 获取用户访问的常用菜单列表。
    /// </summary>
    /// <param name="userName">用户名。</param>
    /// <param name="size">常用菜单数量。</param>
    /// <returns>常用菜单列表。</returns>
    public virtual async Task<List<string>> GetVisitMenuIdsAsync(string userName, int size)
    {
        var logs = await Query<SysLog>().Where(d => d.Type == $"{LogType.Page}" && d.CreateBy == userName)
                                        .GroupBy(d => d.Target)
                                        .Select(d => new CountInfo { Field1 = d.Target, TotalCount = DbFunc.Count() })
                                        .ToListAsync();
        logs = logs?.OrderByDescending(f => f.TotalCount).Take(size).ToList();
        return logs?.Select(l => l.Field1).ToList();
    }

    /// <summary>
    /// 异步查询用户分页数据。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页查询结果。</returns>
    public virtual async Task<PagingResult<SysUser>> QueryUsersAsync(PagingCriteria criteria)
    {
        var sql = $@"select a.*,b.{FormatName("Name")} as {FormatName("Department")} 
from {FormatName("SysUser")} a 
left join {FormatName("SysOrganization")} b on b.{FormatName("Id")}=a.{FormatName("OrgNo")} 
where a.{FormatName("CompNo")}=@CompNo and a.{FormatName("UserName")}<>'admin'";
        var orgNoId = nameof(SysUser.OrgNo);
        var orgNo = criteria.GetParameter<string>(orgNoId);
        if (!string.IsNullOrWhiteSpace(orgNo))
        {
            var org = await QueryByIdAsync<SysOrganization>(orgNo);
            if (org != null && org.Code != User?.CompNo)
                criteria.SetQuery(orgNoId, QueryType.Equal, orgNo);
            else
                criteria.RemoveQuery(orgNoId);
        }
        criteria.Fields[nameof(SysUser.Name)] = $"a.{FormatName("Name")}";
        return await QueryPageAsync<SysUser>(sql, criteria);
        //return await db.Select<SysUser>()
        //               .LeftJoin<SysOrganization>((a, b) => b.Id == a.OrgNo)
        //               .Select<SysOrganization>(d => d.Name, "Department")
        //               .Where(d => d.CompNo == db.User.CompNo && d.UserName != "admin")
        //               .ToPageAsync(criteria);
    }

    /// <summary>
    /// 异步获取用户角色模块ID列表。
    /// </summary>
    /// <param name="userId">用户ID。</param>
    /// <returns>用户角色模块ID列表。</returns>
    public virtual Task<List<string>> GetRoleModuleIdsAsync(string userId)
    {
        var sql = $@"select a.{FormatName("ModuleId")} from {FormatName("SysRoleModule")} a 
where a.{FormatName("RoleId")} in (select {FormatName("RoleId")} from {FormatName("SysUserRole")} where {FormatName("UserId")}=@UserId)
  and exists (select 1 from {FormatName("SysRole")} where {FormatName("Id")}=a.{FormatName("RoleId")} and {FormatName("Enabled")}='True')";
        return ScalarsAsync<string>(sql, new { UserId = userId });
    }
}