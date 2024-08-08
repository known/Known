namespace Known.Services;

class DataRepository
{
    //Log
    internal static Task<List<CountInfo>> GetLogCountsAsync(Database db, string userName, string logType)
    {
        var sql = "select Target as Field1,count(*) as TotalCount from SysLog where CreateBy=@userName and Type=@logType group by Target";
        return db.QueryListAsync<CountInfo>(sql, new { userName, logType });
    }

    //File
    internal static Task<List<SysFile>> GetFilesAsync(Database db, string[] bizIds)
    {
        var idTexts = new List<string>();
        var paramters = new Dictionary<string, object>();
        for (int i = 0; i < bizIds.Length; i++)
        {
            idTexts.Add($"BizId=@id{i}");
            paramters.Add($"id{i}", bizIds[i]);
        }

        var idText = string.Join(" or ", idTexts.ToArray());
        var sql = $"select * from SysFile where {idText} order by CreateTime";
        return db.QueryListAsync<SysFile>(sql, paramters);
    }

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

    internal static Task<List<SysUser>> GetUsersByRoleAsync(Database db, string roleName)
    {
        var sql = $"select * from SysUser where Role like '%{roleName}%'";
        return db.QueryListAsync<SysUser>(sql);
    }

    //Account
    internal static Task<List<string>> GetUserModuleIdsAsync(Database db, string userId)
    {
        var sql = @"select a.ModuleId from SysRoleModule a 
where a.RoleId in (select RoleId from SysUserRole where UserId=@userId)
  and exists (select 1 from SysRole where Id=a.RoleId and Enabled='True')";
        return db.ScalarsAsync<string>(sql, new { userId });
    }

    //Flow
    internal static Task<List<SysFlow>> GetFlowsAsync(Database db, string bizIds)
    {
        if (string.IsNullOrWhiteSpace(bizIds))
            return null;

        var ids = bizIds.Replace(",", "','");
        var sql = $"select * from SysFlow where BizId in ('{ids}')";
        return db.QueryListAsync<SysFlow>(sql);
    }
}