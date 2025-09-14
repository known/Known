namespace Known.Extensions;

static class SysUserExtension
{
    internal static async Task<string> GetUserDataAsync(this Database db, string id)
    {
        var user = await db.QueryByIdAsync<SysUser>(id);
        return user?.Data;
    }

    internal static async Task<UserInfo> GetSysUserAsync(this Database db, string userName)
    {
        var user = await db.QueryAsync<SysUser>(d => d.UserName == userName);
        return await db.ToUserInfo(user);
    }

    internal static async Task<UserInfo> GetSysUserAsync(this Database db, string userName, string password)
    {
        var user = await db.QueryAsync<SysUser>(d => d.UserName == userName && d.Password == password);
        return await db.ToUserInfo(user);
    }

    internal static async Task<UserInfo> GetSysUserByIdAsync(this Database db, string id)
    {
        var user = await db.QueryByIdAsync<SysUser>(id);
        return await db.ToUserInfo(user);
    }

    internal static Task<List<UserInfo>> GetSysUsersByRoleAsync(this Database db, string role)
    {
        return db.Query<SysUser>().ToListAsync<UserInfo>(d => d.Role.Contains(role));
    }

    internal static async Task<Result> AddSysUserAsync(this Database db, UserDataInfo info)
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

    internal static async Task<Result> SaveSysUserAsync(this Database db, Context context, UserInfo info)
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

    internal static async Task<Result> SyncSysUserAsync(this Database db, UserInfo user)
    {
        var model = await db.QueryAsync<SysUser>(d => d.UserName == user.UserName);
        if (model == null)
        {
            model = new SysUser
            {
                OrgNo = user.OrgNo,
                UserName = user.UserName,
                //Password = user.Password,
                Name = user.Name,
                Gender = user.Gender,
                Phone = user.Phone,
                Mobile = user.Mobile,
                Email = user.Email,
                Enabled = true,
                Role = user.Role
            };
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                model.Password = Utils.ToMd5(model.Password);
            }
            else
            {
                var info = await db.GetSystemAsync();
                model.Password = Utils.ToMd5(info?.UserDefaultPwd);
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

    internal static async Task<Result> UpdateUserAvatarAsync(this Database db, AvatarInfo info)
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

    internal static async Task<Result> UpdateUserPasswordAsync(this Database db, PwdFormInfo info)
    {
        var entity = await db.QueryByIdAsync<SysUser>(info.UserId);
        if (entity == null)
            return Result.Error(Language.TipNoUser);

        var oldPwd = Utils.ToMd5(info.OldPwd);
        if (entity.Password != oldPwd)
            return Result.Error(AdminLanguage.TipCurPwdInvalid);

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

        var sys = await db.GetUserSystemAsync();
        info.IsTenant = user.CompNo != sys?.CompNo;
        info.AppName = sys?.AppName;
        if (info.IsAdmin())
            user.AppId = Config.App.Id;
        info.CompName = sys?.CompName;
        if (!string.IsNullOrEmpty(info.OrgNo))
        {
            var org = await db.QueryAsync<SysOrganization>(d => d.Id == info.OrgNo || (d.CompNo == info.CompNo && d.Code == info.OrgNo));
            var orgName = org?.Name ?? info.CompName;
            info.OrgName = orgName;
            if (string.IsNullOrEmpty(info.CompName))
                info.CompName = orgName;
        }
        return info;
    }

    //internal const string UTOperation = "Operation";
    internal const string UMTypeReceive = "Receive";
    internal const string UMTypeSend = "Send";
    internal const string UMTypeDelete = "Delete";
    internal const string UMLGeneral = "General";
    internal const string UMLUrgent = "Urgent";
    internal const string UMStatusRead = "Read";
    internal const string UMStatusUnread = "Unread";
    //internal const string NSPublished = "Published";

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
        var level = isUrgent ? UMLUrgent : UMLGeneral;
        return SendMessageAsync(db, user, level, toUser, subject, content, filePath, bizId);
    }

    private static Task SendMessageAsync(Database db, UserInfo user, string level, string toUser, string subject, string content, string filePath = null, string bizId = null)
    {
        var model = new SysMessage
        {
            UserId = toUser,
            Type = UMTypeReceive,
            MsgBy = user.Name,
            MsgLevel = level,
            Subject = subject,
            Content = content,
            FilePath = filePath,
            IsHtml = true,
            Status = UMStatusUnread,
            BizId = bizId
        };
        return db.SaveAsync(model);
    }
}