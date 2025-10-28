namespace Known.Extensions;

/// <summary>
/// 用户数据扩展类。
/// </summary>
public static class UserExtension
{
    internal static async Task<string> GetUserOrgNameAsync(this Database db, UserInfo info)
    {
        var org = await db.QueryAsync<SysOrganization>(d => d.Id == info.OrgNo || (d.CompNo == info.CompNo && d.Code == info.OrgNo));
        return org?.Name;
    }

    /// <summary>
    /// 异步根据用户名获取用户信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="userName">用户名。</param>
    /// <returns>用户信息。</returns>
    public static async Task<UserInfo> GetUserAsync(this Database db, string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return default;

        userName = userName.ToLower();
        var user = await db.QueryAsync<SysUser>(d => d.UserName == userName);
        return await db.ToUserInfo(user);
    }

    /// <summary>
    /// 根据用户名和密码获取用户信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="userName">用户名。</param>
    /// <param name="password">密码。</param>
    /// <returns></returns>
    public static async Task<UserInfo> GetUserAsync(this Database db, string userName, string password)
    {
        userName = userName.ToLower();
        var user = await db.QueryAsync<SysUser>(d => d.UserName == userName && d.Password == password);
        return await db.ToUserInfo(user);
    }

    /// <summary>
    /// 异步根据用户ID获取用户信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="id">用户ID。</param>
    /// <returns>用户信息。</returns>
    public static async Task<UserInfo> GetUserByIdAsync(this Database db, string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return default;

        var user = await db.QueryByIdAsync<SysUser>(id);
        return await db.ToUserInfo(user);
    }

    /// <summary>
    /// 异步根据用户姓名获取用户信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="name">用户姓名。</param>
    /// <returns>用户信息。</returns>
    public static async Task<UserInfo> GetUserByNameAsync(this Database db, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return default;

        var user = await db.QueryAsync<SysUser>(d => d.Name == name);
        return await db.ToUserInfo(user);
    }

    /// <summary>
    /// 异步获取角色用户列表。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="roleName">角色名称。</param>
    /// <returns>用户列表。</returns>
    public static Task<List<UserInfo>> GetUsersByRoleAsync(this Database db, string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            return Task.FromResult(new List<UserInfo>());

        return db.Query<SysUser>().ToListAsync<UserInfo>(d => d.Role.Contains(roleName));
    }

    /// <summary>
    /// 异步添加系统用户。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">系统用户对象。</param>
    /// <returns></returns>
    public static async Task<Result> AddUserAsync(this Database db, SysUser info)
    {
        var model = new SysUser
        {
            UserName = info.UserName.ToLower(),
            Name = info.UserName,
            EnglishName = info.UserName,
            Password = Utils.ToMd5(info.Password),
            FirstLoginTime = DateTime.Now,
            FirstLoginIP = info.FirstLoginIP,
            LastLoginTime = DateTime.Now,
            LastLoginIP = info.LastLoginIP
        };
        await db.SaveAsync(model);
        return Result.Success("添加成功！");
    }

    /// <summary>
    /// 异步保存系统用户。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="context">系统上下文。</param>
    /// <param name="info">系统用户对象。</param>
    /// <returns></returns>
    public static async Task<Result> SaveUserAsync(this Database db, Context context, UserInfo info)
    {
        var model = await db.QueryByIdAsync<SysUser>(info.Id);
        if (model == null)
            return Result.Error(Language.TipNoUser);

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

        var vr = model.Validate(context);
        if (!vr.IsValid)
            return vr;

        await db.SaveAsync(model);
        return Result.Success(Language.SaveSuccess);
    }

    /// <summary>
    /// 异步同步系统用户到框架用户表。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">系统用户对象。</param>
    /// <returns></returns>
    public static async Task<Result> SyncUserAsync(this Database db, SysUser info)
    {
        var model = await db.QueryAsync<SysUser>(d => d.UserName == info.UserName);
        if (model == null)
        {
            model = new SysUser
            {
                OrgNo = info.OrgNo,
                UserName = info.UserName,
                Password = info.Password,
                Name = info.Name,
                Gender = info.Gender,
                Phone = info.Phone,
                Mobile = info.Mobile,
                Email = info.Email,
                Enabled = true,
                Role = info.Role
            };
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                model.Password = Utils.ToMd5(model.Password);
            }
            else
            {
                var sys = await db.GetSystemAsync();
                model.Password = Utils.ToMd5(sys?.UserDefaultPwd);
            }
            await db.SaveAsync(model);
            var role = await db.QueryAsync<SysRole>(d => d.CompNo == model.CompNo && d.Name == model.Role);
            if (role != null)
                await db.InsertAsync(new SysUserRole { UserId = model.Id, RoleId = role.Id });
        }
        else
        {
            model.Enabled = true;
            await db.SaveAsync(model);
        }
        return Result.Success("同步成功！");
    }

    /// <summary>
    /// 异步修改用户头像。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">用户头像信息。</param>
    /// <returns></returns>
    public static async Task<Result> UpdateAvatarAsync(this Database db, AvatarInfo info)
    {
        var entity = await db.QueryAsync<SysUser>(d => d.Id == info.UserId);
        if (entity == null)
            return Result.Error(Language.TipNoUser);

        var attach = new AttachFile(info.File, "Avatars");
        attach.FilePath = $"Avatars/{entity.Id}{attach.ExtName}";
        await attach.SaveAsync();

        var url = Config.GetFileUrl(attach.FilePath);
        entity.SetExtension(nameof(UserInfo.AvatarUrl), url);
        await db.SaveAsync(entity);
        return Result.Success(Language.SaveSuccess, url);
    }

    /// <summary>
    /// 异步修改用户密码。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">密码表单信息。</param>
    /// <returns></returns>
    public static async Task<Result> UpdatePasswordAsync(this Database db, PwdFormInfo info)
    {
        var entity = await db.QueryByIdAsync<SysUser>(info.UserId);
        if (entity == null)
            return Result.Error(Language.TipNoUser);

        var oldPwd = Utils.ToMd5(info.OldPwd);
        if (entity.Password != oldPwd)
            return Result.Error(Language.TipCurPwdInvalid);

        entity.Password = Utils.ToMd5(info.NewPwd);
        await db.SaveAsync(entity);
        return Result.Success("修改成功！", entity.Id);
    }

    private static async Task<UserInfo> ToUserInfo(this Database db, SysUser user)
    {
        if (user == null)
            return null;

        var info = Utils.MapTo<UserInfo>(user);
        var avatarUrl = user.GetExtension<string>(nameof(UserInfo.AvatarUrl));
        if (string.IsNullOrWhiteSpace(avatarUrl))
            avatarUrl = user.Gender == "Female" ? "img/face2.png" : "img/face1.png";
        info.AvatarUrl = avatarUrl;

        var sys = await db.GetUserSystemAsync(info);
        info.IsTenant = user.CompNo != sys?.CompNo;
        info.AppName = sys?.AppName;
        if (info.IsAdmin())
            user.AppId = Config.App.Id;
        info.CompName = sys?.CompName;
        if (!string.IsNullOrEmpty(info.OrgNo))
        {
            info.OrgName = await db.GetUserOrgNameAsync(info);
            if (string.IsNullOrEmpty(info.CompName))
                info.CompName = info.OrgName;
        }
        return info;
    }
}