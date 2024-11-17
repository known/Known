namespace Known.Extensions;

/// <summary>
/// 用户数据扩展类。
/// </summary>
public static class UserExtension
{
    /// <summary>
    /// 异步获取角色用户列表。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="roleName">角色名称。</param>
    /// <returns>用户列表。</returns>
    public static Task<List<UserInfo>> GetUsersByRoleAsync(this Database db, string roleName)
    {
        return db.Query<SysUser>().Where(d => d.Role.Contains(roleName)).ToListAsync<UserInfo>();
    }

    /// <summary>
    /// 异步同步系统用户到框架用户表。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="user">系统用户对象。</param>
    /// <returns></returns>
    public static async Task SyncUserAsync(this Database db, SysUser user)
    {
        var model = await db.QueryAsync<SysUser>(d => d.UserName == user.UserName);
        if (model == null)
        {
            model = new SysUser
            {
                OrgNo = user.OrgNo,
                UserName = user.UserName,
                Password = Utils.ToMd5(user.Password),
                Name = user.Name,
                Gender = user.Gender,
                Phone = user.Phone,
                Mobile = user.Mobile,
                Email = user.Email,
                Enabled = true,
                Role = user.Role
            };
            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                model.Password = Utils.ToMd5(user.Password);
            }
            else
            {
                var info = await db.GetSystemAsync();
                model.Password = Utils.ToMd5(info?.UserDefaultPwd);
            }
            await db.SaveAsync(model);
            var role = await db.QueryAsync<SysRole>(d => d.CompNo == user.CompNo && d.Name == user.Role);
            if (role != null)
                await db.InsertAsync(new SysUserRole { UserId = model.Id, RoleId = role.Id });
        }
    }

    internal static async Task<UserInfo> GetUserAsync(this Database db, string userName)
    {
        var user = await db.QueryAsync<SysUser>(d => d.UserName == userName);
        return user.ToUserInfo();
    }

    internal static async Task<UserInfo> GetUserByIdAsync(this Database db, string userId)
    {
        var user = await db.QueryAsync<SysUser>(d => d.Id == userId);
        return user.ToUserInfo();
    }

    internal static async Task<UserInfo> GetUserAsync(this Database db, string userName, string password)
    {
        var user = await db.QueryAsync<SysUser>(d => d.UserName == userName && d.Password == password);
        return await db.GetUserInfoAsync(user);
    }

    internal static async Task<UserInfo> GetUserInfoAsync(this Database db, SysUser user)
    {
        if (user == null)
            return null;

        var info = user.ToUserInfo();
        await SetUserInfoAsync(db, info);
        return info;
    }

    private static async Task SetUserInfoAsync(Database db, UserInfo user)
    {
        var info = await db.GetSystemAsync();
        user.IsTenant = user.CompNo != info?.CompNo;
        user.AppName = info?.AppName;
        if (user.IsAdmin())
            user.AppId = Config.App.Id;
        user.CompName = info?.CompName;
        if (!string.IsNullOrEmpty(user.OrgNo))
        {
            var org = await db.QueryAsync<SysOrganization>(d => d.CompNo == user.CompNo && d.Code == user.OrgNo);
            var orgName = org?.Name ?? user.CompName;
            user.OrgName = orgName;
            if (string.IsNullOrEmpty(user.CompName))
                user.CompName = orgName;
        }
    }

    internal static async Task<Result> SaveUserAsync(this Database db, UserInfo info)
    {
        var model = await db.QueryByIdAsync<SysUser>(info.Id);
        if (model == null)
            return Result.Error(db.Context.Language["Tip.NoUser"]);

        model.Name = info.Name;
        model.EnglishName = info.EnglishName;
        model.Gender = info.Gender;
        model.Phone = info.Phone;
        model.Mobile = info.Mobile;
        model.Email = info.Email;
        model.Note = info.Note;
        if (!info.FirstLoginTime.HasValue)
        {
            model.FirstLoginTime = info.FirstLoginTime;
            model.FirstLoginIP = info.FirstLoginIP;
        }
        model.LastLoginTime = info.LastLoginTime;
        model.LastLoginIP = info.LastLoginIP;

        var vr = model.Validate(db.Context);
        if (!vr.IsValid)
            return vr;

        await db.SaveAsync(model);
        return Result.Success("保存成功！");
    }

    internal static UserInfo ToUserInfo(this SysUser user)
    {
        if (user == null)
            return null;

        var info = Utils.MapTo<UserInfo>(user);
        var avatarUrl = user.GetExtension<string>(nameof(UserInfo.AvatarUrl));
        if (string.IsNullOrWhiteSpace(avatarUrl))
            avatarUrl = user.Gender == "Female" ? "img/face2.png" : "img/face1.png";
        info.AvatarUrl = avatarUrl;
        return info;
    }
}