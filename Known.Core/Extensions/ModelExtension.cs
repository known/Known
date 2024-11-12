namespace Known.Core.Extensions;

/// <summary>
/// 数据模型扩展类。
/// </summary>
public static class ModelExtension
{
    #region User
    internal static async Task<UserInfo> ToUserAsync(this SysUser entity, Database db)
    {
        var user = Utils.MapTo<UserInfo>(entity);
        var avatarUrl = entity.GetExtension<string>(nameof(UserInfo.AvatarUrl));
        if (string.IsNullOrWhiteSpace(avatarUrl))
            avatarUrl = entity.Gender == "Female" ? "img/face2.png" : "img/face1.png";
        user.AvatarUrl = avatarUrl;
        await user.SetUserInfoAsync(db);
        //await user.SetUserWeixinAsync(db);
        return user;
    }

    private static async Task SetUserInfoAsync(this UserInfo user, Database db)
    {
        var info = await ConfigHelper.GetSystemAsync(db);
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
    #endregion

    #region Module
    internal static List<MenuInfo> ToMenus(this List<SysModule> modules, bool isAdmin)
    {
        if (modules == null || modules.Count == 0)
            return [];

        return modules.Select(m => new MenuInfo(m, isAdmin)).ToList();
    }

    internal static void RemoveModule(this List<SysModule> modules, string code)
    {
        var module = modules.FirstOrDefault(m => m.Code == code);
        if (module != null)
            modules.Remove(module);
    }
    #endregion
}