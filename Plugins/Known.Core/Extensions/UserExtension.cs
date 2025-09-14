namespace Known.Extensions;

/// <summary>
/// 用户数据扩展类。
/// </summary>
public static class UserExtension
{
    /// <summary>
    /// 异步根据用户名获取用户信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="userName">用户名。</param>
    /// <returns>用户信息。</returns>
    public static Task<UserInfo> GetUserAsync(this Database db, string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return Task.FromResult(default(UserInfo));

        userName = userName.ToLower();
        return db.Query<SysUser>().FirstAsync<UserInfo>(d => d.UserName == userName);
    }

    /// <summary>
    /// 异步根据用户ID获取用户信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="id">用户ID。</param>
    /// <returns>用户信息。</returns>
    public static Task<UserInfo> GetUserByIdAsync(this Database db, string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return Task.FromResult(default(UserInfo));

        return db.Query<SysUser>().FirstAsync<UserInfo>(d => d.Id == id);
    }

    /// <summary>
    /// 异步根据用户姓名获取用户信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="name">用户姓名。</param>
    /// <returns>用户信息。</returns>
    public static Task<UserInfo> GetUserByNameAsync(this Database db, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Task.FromResult(default(UserInfo));

        return db.Query<SysUser>().FirstAsync<UserInfo>(d => d.Name == name);
    }

    /// <summary>
    /// 异步获取角色用户列表。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="roleName">角色名称。</param>
    /// <returns>用户列表。</returns>
    public static Task<List<UserInfo>> GetUsersByRoleAsync(this Database db, string roleName)
    {
        return db.Query<SysUser>().ToListAsync<UserInfo>(d => d.Role.Contains(roleName));
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
                Password = user.Password,
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
            //var role = await db.QueryAsync<SysRole>(d => d.CompNo == model.CompNo && d.Name == model.Role);
            //if (role != null)
            //    await db.InsertAsync(new SysUserRole { UserId = model.Id, RoleId = role.Id });
        }
        else
        {
            model.Enabled = true;
            await db.SaveAsync(model);
        }
    }

    /// <summary>
    /// 发送站内消息。
    /// </summary>
    /// <param name="user">用户对象。</param>
    /// <param name="db">数据库对象。</param>
    /// <param name="toUser">站内收件人。</param>
    /// <param name="subject">消息主题。</param>
    /// <param name="content">消息内容。</param>
    /// <param name="isUrgent">是否紧急。</param>
    /// <param name="filePath">附件路径。</param>
    /// <param name="bizId">关联业务数据ID。</param>
    /// <returns></returns>
    public static Task SendMessageAsync(this UserInfo user, Database db, string toUser, string subject, string content, bool isUrgent = false, string filePath = null, string bizId = null)
    {
        var level = isUrgent ? Constant.UMLUrgent : Constant.UMLGeneral;
        return SendMessageAsync(db, user, level, toUser, subject, content, filePath, bizId);
    }

    private static Task SendMessageAsync(Database db, UserInfo user, string level, string toUser, string subject, string content, string filePath = null, string bizId = null)
    {
        var model = new SysMessage
        {
            UserId = toUser,
            Type = Constant.UMTypeReceive,
            MsgBy = user.Name,
            MsgLevel = level,
            Subject = subject,
            Content = content,
            FilePath = filePath,
            IsHtml = true,
            Status = Constant.UMStatusUnread,
            BizId = bizId
        };
        return db.SaveAsync(model);
    }

    internal static async Task<UserInfo> GetUserInfoAsync(this Database db, string userName)
    {
        var user = await db.QueryAsync<SysUser>(d => d.UserName == userName);
        return user.ToUserInfo();
    }

    internal static async Task<UserInfo> GetUserInfoAsync(this Database db, string userName, string password)
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
        var info = await db.GetUserSystemAsync();
        user.IsTenant = user.CompNo != info?.CompNo;
        user.AppName = info?.AppName;
        if (user.IsAdmin())
            user.AppId = Config.App.Id;
        user.CompName = info?.CompName;
        if (!string.IsNullOrEmpty(user.OrgNo))
        {
            //var org = await db.QueryAsync<SysOrganization>(d => d.Id == user.OrgNo || (d.CompNo == user.CompNo && d.Code == user.OrgNo));
            //var orgName = org?.Name ?? user.CompName;
            //user.OrgName = orgName;
            //if (string.IsNullOrEmpty(user.CompName))
            //    user.CompName = orgName;
        }
    }

    internal static async Task<Result> SaveUserAsync(this Database db, Context context, UserInfo info)
    {
        var model = await db.QueryByIdAsync<SysUser>(info.Id);
        if (model == null)
            return Result.Error(CoreLanguage.TipNoUser);

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