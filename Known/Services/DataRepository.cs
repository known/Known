namespace Known.Services;

class DataRepository
{
    //User
    internal static async Task<PagingResult<SysUser>> QueryUsersAsync(Database db, PagingCriteria criteria)
    {
        var sql = @"
select a.*,b.Name as Department 
from SysUser a 
left join SysOrganization b on b.Id=a.OrgNo 
where a.AppId=@AppId and a.CompNo=@CompNo and a.UserName<>'admin'";
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
    }

    //Account
    internal static Task<List<string>> GetUserModuleIdsAsync(Database db, string userId)
    {
        var sql = @"select a.ModuleId from SysRoleModule a 
where a.RoleId in (select RoleId from SysUserRole where UserId=@userId)
  and exists (select 1 from SysRole where Id=a.RoleId and Enabled='True')";
        return db.ScalarsAsync<string>(sql, new { userId });
    }
}