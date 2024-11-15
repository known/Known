namespace Known;

/// <summary>
/// 平台操作类，提供框架数据的常用操作方法。
/// </summary>
public sealed class Platform
{
    private Platform() { }

    #region User
    /// <summary>
    /// 异步根据用户名获取用户信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="userName">用户名。</param>
    /// <returns>用户信息。</returns>
    public static Task<UserInfo> GetUserAsync(Database db, string userName)
    {
        return db.QueryAsync<UserInfo>(d => d.UserName == userName);
    }

    /// <summary>
    /// 异步根据用户ID获取用户信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="id">用户ID。</param>
    /// <returns>用户信息。</returns>
    public static Task<UserInfo> GetUserByIdAsync(Database db, string id)
    {
        return db.QueryAsync<UserInfo>(d => d.Id == id);
    }

    /// <summary>
    /// 异步根据用户姓名获取用户信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="name">用户姓名。</param>
    /// <returns>用户信息。</returns>
    public static Task<UserInfo> GetUserByNameAsync(Database db, string name)
    {
        return db.QueryAsync<UserInfo>(d => d.Name == name);
    }

    /// <summary>
    /// 异步获取角色用户列表。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="roleName">角色名称。</param>
    /// <returns>用户列表。</returns>
    public static Task<List<UserInfo>> GetUsersByRoleAsync(Database db, string roleName)
    {
        return db.QueryListAsync<UserInfo>(d => d.Role.Contains(roleName));
    }

    /// <summary>
    /// 异步同步系统用户到框架用户表。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="user">系统用户对象。</param>
    /// <returns></returns>
    public static async Task SyncUserAsync(Database db, SysUser user)
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
    #endregion

    #region File
    /// <summary>
    /// 异步获取系统附件信息列表。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="bizId">附件业务数据ID。</param>
    /// <returns>系统附件信息列表。</returns>
    public static Task<List<AttachInfo>> GetFilesAsync(Database db, string bizId)
    {
        return FileService.GetFilesAsync(db, bizId);
    }

    /// <summary>
    /// 异步添加系统附件信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="files">表单附件列表。</param>
    /// <param name="bizId">附件业务数据ID。</param>
    /// <param name="bizType">附件业务类型。</param>
    /// <returns>系统附件信息列表。</returns>
    public static Task<List<AttachInfo>> AddFilesAsync(Database db, List<AttachFile> files, string bizId, string bizType)
    {
        return FileService.AddFilesAsync(db, files, bizId, bizType);
    }

    /// <summary>
    /// 异步删除系统附件实体。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="id">附件ID。</param>
    /// <returns></returns>
    public static Task DeleteFileAsync(Database db, string id)
    {
        return db.DeleteAsync<SysFile>(id);
    }

    /// <summary>
    /// 物理删除附件。
    /// </summary>
    /// <param name="filePaths">附件路径列表。</param>
    public static void DeleteFiles(List<string> filePaths)
    {
        filePaths.ForEach(AttachFile.DeleteFile);
    }

    /// <summary>
    /// 异步删除系统附件表数据。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="bizId">附件业务数据ID。</param>
    /// <param name="oldFiles">要物理删除的附件路径列表。</param>
    /// <returns></returns>
    public static Task DeleteFilesAsync(Database db, string bizId, List<string> oldFiles)
    {
        return FileService.DeleteFilesAsync(db, bizId, oldFiles);
    }
    #endregion

    #region Weixin
    /// <summary>
    /// 异步获取系统用户绑定的微信信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="user">系统用户。</param>
    /// <returns>微信信息。</returns>
    public static Task<SysWeixin> GetWeixinAsync(Database db, UserInfo user)
    {
        return WeixinService.GetWeixinByUserIdAsync(db, user.Id);
    }

    /// <summary>
    /// 异步发送微信模板消息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">模板业务信息。</param>
    /// <returns>发送结果。</returns>
    public static async Task<Result> SendTemplateMessageAsync(Database db, WeixinTemplateInfo info)
    {
        var task = WeixinHelper.CreateTask(info);
        await db.CreateTaskAsync(task);
        TaskHelper.NotifyRun(task.Type);
        return Result.Success("Task saved！");
    }
    #endregion

    #region WorkFlow
    /// <summary>
    /// 异步创建系统工作流。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="info">业务工作流信息。</param>
    /// <returns></returns>
    public static Task CreateFlowAsync(Database db, FlowBizInfo info)
    {
        return FlowService.CreateFlowAsync(db, info);
    }

    /// <summary>
    /// 异步删除工作流及其日志。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="bizId">工作流业务数据ID。</param>
    /// <returns></returns>
    public static Task DeleteFlowAsync(Database db, string bizId)
    {
        return FlowService.DeleteFlowAsync(db, bizId);
    }

    /// <summary>
    /// 异步添加工作流日志信息。
    /// </summary>
    /// <param name="db">数据库对象。</param>
    /// <param name="bizId">工作流业务数据ID。</param>
    /// <param name="stepName">流程步骤名称。</param>
    /// <param name="result">流程操作结果。</param>
    /// <param name="note">操作备注。</param>
    /// <param name="time">操作时间。</param>
    /// <returns></returns>
    public static Task AddFlowLogAsync(Database db, string bizId, string stepName, string result, string note, DateTime? time = null)
    {
        return FlowService.AddFlowLogAsync(db, bizId, stepName, result, note, time);
    }
    #endregion
}