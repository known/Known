namespace Known.Admin.Extensions;

/// <summary>
/// 后台管理平台服务扩展类。
/// </summary>
public static class PlatformExtension
{
    #region System
    /// <summary>
    /// 异步获取系统信息。
    /// </summary>
    /// <param name="platform">平台服务实例。</param>
    /// <param name="db">数据库对象。</param>
    /// <returns>系统信息。</returns>
    public static Task<SystemInfo> GetSystemAsync(this IPlatformService platform, Database db)
    {
        return ConfigHelper.GetSystemAsync(db);
    }
    #endregion

    #region User
    /// <summary>
    /// 异步获取角色用户列表。
    /// </summary>
    /// <param name="platform">平台服务实例。</param>
    /// <param name="db">数据库对象。</param>
    /// <param name="roleName">角色名称。</param>
    /// <returns>用户列表。</returns>
    public static Task<List<SysUser>> GetUsersByRoleAsync(this IPlatformService platform, Database db, string roleName)
    {
        return db.QueryListAsync<SysUser>(d => d.Role.Contains(roleName));
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
            var info = await ConfigHelper.GetSystemAsync(db);
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

    #region File
    /// <summary>
    /// 异步获取系统附件信息列表。
    /// </summary>
    /// <param name="platform">平台服务实例。</param>
    /// <param name="db">数据库对象。</param>
    /// <param name="bizId">附件业务数据ID。</param>
    /// <returns>系统附件信息列表。</returns>
    public static Task<List<SysFile>> GetFilesAsync(this IPlatformService platform, Database db, string bizId)
    {
        return FileService.GetFilesAsync(db, bizId);
    }

    /// <summary>
    /// 异步添加系统附件信息。
    /// </summary>
    /// <param name="platform">平台服务实例。</param>
    /// <param name="db">数据库对象。</param>
    /// <param name="files">表单附件列表。</param>
    /// <param name="bizId">附件业务数据ID。</param>
    /// <param name="bizType">附件业务类型。</param>
    /// <returns>系统附件信息列表。</returns>
    public static Task<List<SysFile>> AddFilesAsync(this IPlatformService platform, Database db, List<AttachFile> files, string bizId, string bizType)
    {
        return FileService.AddFilesAsync(db, files, bizId, bizType);
    }
    #endregion
}