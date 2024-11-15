namespace Known;

/// <summary>
/// 后台管理平台服务扩展类。
/// </summary>
public static class PlatformExtension
{
    #region User
    /// <summary>
    /// 异步获取角色用户列表。
    /// </summary>
    /// <param name="platform">平台服务实例。</param>
    /// <param name="db">数据库对象。</param>
    /// <param name="roleName">角色名称。</param>
    /// <returns>用户列表。</returns>
    public static Task<List<UserInfo>> GetUsersByRoleAsync(this IPlatformService platform, Database db, string roleName)
    {
        return db.QueryListAsync<UserInfo>(d => d.Role.Contains(roleName));
    }

    /// <summary>
    /// 异步同步系统用户到框架用户表。
    /// </summary>
    /// <param name="platform">平台服务实例。</param>
    /// <param name="db">数据库对象。</param>
    /// <param name="user">系统用户对象。</param>
    /// <returns></returns>
    public static async Task SyncUserAsync(this IPlatformService platform, Database db, SysUser user)
    {
        var model = await db.QueryAsync<SysUser>(d => d.UserName == user.UserName);
        if (model == null)
        {
            model = new SysUser
            {
                OrgNo = user.OrgNo,
                UserName = user.UserName,
                Name = user.Name,
                Gender = user.Gender,
                Phone = user.Phone,
                Mobile = user.Mobile,
                Email = user.Email,
                Enabled = true,
                Role = user.Role
            };
            var info = await platform.GetSystemAsync(db);
            if (!string.IsNullOrWhiteSpace(user.Password))
                model.Password = Utils.ToMd5(user.Password);
            else if (info != null)
                model.Password = Utils.ToMd5(info.UserDefaultPwd);
            await db.SaveAsync(model);
            var role = await db.QueryAsync<SysRole>(d => d.CompNo == user.CompNo && d.Name == user.Role);
            if (role != null)
                await db.InsertAsync(new SysUserRole { UserId = model.Id, RoleId = role.Id });
        }
    }
    #endregion

    #region Log
    /// <summary>
    /// 异步获取常用功能菜单信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="userName">用户名。</param>
    /// <param name="size">Top数量。</param>
    /// <returns>功能菜单信息。</returns>
    public static async Task<List<string>> GetVisitMenuIdsAsync(this Database db, string userName, int size)
    {
        var logs = await db.Query<SysLog>()
                           .Where(d => d.Type == $"{LogType.Page}" && d.CreateBy == userName)
                           .GroupBy(d => d.Target)
                           .Select(d => new CountInfo { Field1 = d.Target, TotalCount = DbFunc.Count() })
                           .ToListAsync();
        logs = logs?.OrderByDescending(f => f.TotalCount).Take(size).ToList();
        return logs?.Select(l => l.Field1).ToList();
    }
    #endregion
}