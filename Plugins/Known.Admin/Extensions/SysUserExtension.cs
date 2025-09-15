namespace Known.Extensions;

static class SysUserExtension
{
    internal static async Task<Result> SyncSysUserAsync(this Database db, UserDataInfo user)
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

    internal static async Task<string> GetUserOrgNameAsync(this Database db, UserInfo info)
    {
        var org = await db.QueryAsync<SysOrganization>(d => d.Id == info.OrgNo || (d.CompNo == info.CompNo && d.Code == info.OrgNo));
        return org?.Name;
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