using System.Collections.Concurrent;
using Known.Entities;
using Known.Helpers;
using Known.Repositories;

namespace Known.Services;

class AuthService : ServiceBase
{
    //Account
    public async Task<Result> SignInAsync(LoginFormInfo info)
    {
        var userName = info.UserName.ToLower();
        var entity = await UserRepository.GetUserAsync(Database, userName, info.Password);
        if (entity == null)
            return Result.Error(Language.LoginNoNamePwd);

        if (!entity.Enabled)
            return Result.Error(Language.LoginDisabled);

        if (!entity.FirstLoginTime.HasValue)
        {
            entity.FirstLoginTime = DateTime.Now;
            entity.FirstLoginIP = info.IPAddress;
        }
        entity.LastLoginTime = DateTime.Now;
        entity.LastLoginIP = info.IPAddress;

        var user = Utils.MapTo<UserInfo>(entity);
        user.Token = Utils.GetGuid();
        await SetUserInfoAsync(user);
        cachedUsers[user.Token] = user;

        var type = LogType.Login;
        if (info.IsMobile)
            type = "APP" + type;

        var database = Database;
        database.User = user;
        return await database.TransactionAsync(Language.Login, async db =>
        {
            await db.SaveAsync(entity);
            await Logger.AddLogAsync(db, type, $"{user.UserName}-{user.Name}", $"IP：{user.LastLoginIP}；所在地：{user.IPName}");
        }, user);
    }

    public async Task<Result> SignOutAsync(string token)
    {
        var user = CurrentUser;
        if (string.IsNullOrWhiteSpace(token))
            token = user.Token;

        if (!string.IsNullOrWhiteSpace(token))
            cachedUsers.TryRemove(token, out UserInfo _);

        await Logger.AddLogAsync(Database, LogType.Logout, $"{user.UserName}-{user.Name}", $"token: {token}");
        return Result.Success("退出成功！");
    }

    internal static ConcurrentDictionary<string, UserInfo> cachedUsers = new();

    //internal static UserInfo GetUserByToken(string token)
    //{
    //    if (string.IsNullOrWhiteSpace(token))
    //        return null;

    //    return cachedUsers.Values.FirstOrDefault(u => u.Token == token);
    //}

    internal static async Task<UserInfo> GetUserAsync(Database db, string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return null;

        userName = userName.Split('-')[0];
        var user = cachedUsers.Values.FirstOrDefault(u => u.UserName == userName);
        if (user != null)
            return user;

        return await UserRepository.GetUserAsync(db, userName);
    }

    public Task<UserInfo> GetUserAsync(string userName) => GetUserAsync(Database, userName);

    public async Task<AdminInfo> GetAdminAsync()
    {
        //CurrentUser.Setting = await UserHelper.GetUserSettingAsync(Database);
        var result = await DictionaryService.RefreshCacheAsync(Database, CurrentUser);
        var admin = new AdminInfo
        {
            AppName = await UserHelper.GetSystemNameAsync(Database),
            MessageCount = await UserRepository.GetMessageCountAsync(Database),
            UserMenus = await UserHelper.GetUserMenusAsync(Database),
            //Codes = result.Data as List<CodeInfo>
        };
        return admin;
    }

    public async Task<Result> UpdateUserAsync(SysUser model)
    {
        if (model == null)
            return Result.Error(Language.NoUser);

        var vr = model.Validate();
        if (!vr.IsValid)
            return vr;

        await Database.SaveAsync(model);
        return Result.Success(Language.XXSuccess.Format(Language.Save), model);
    }

    public async Task<Result> UpdatePasswordAsync(PwdFormInfo info)
    {
        var user = CurrentUser;
        if (user == null)
            return Result.Error(Language.NoLogin);

        var errors = new List<string>();
        if (string.IsNullOrEmpty(info.OldPwd))
            errors.Add("当前密码不能为空！");
        if (string.IsNullOrEmpty(info.NewPwd))
            errors.Add("新密码不能为空！");
        if (string.IsNullOrEmpty(info.NewPwd1))
            errors.Add("确认新密码不能为空！");
        if (info.NewPwd != info.NewPwd1)
            errors.Add("两次密码输入不一致！");

        if (errors.Count > 0)
            return Result.Error(string.Join(Environment.NewLine, errors.ToArray()));

        var entity = await UserRepository.GetUserAsync(Database, user.UserName, info.OldPwd);
        if (entity == null)
            return Result.Error("当前密码不正确！");

        entity.Password = Utils.ToMd5(info.NewPwd);
        await Database.SaveAsync(entity);
        return Result.Success(Language.XXSuccess.Format(Language.Update), entity.Id);
    }

    private async Task SetUserInfoAsync(UserInfo user)
    {
        var sys = await GetConfigAsync<SystemInfo>(Database, SystemService.KeySystem);
        user.IsTenant = user.CompNo != sys.CompNo;
        user.AppName = Config.App.Name;
        if (user.IsAdmin)
            user.AppId = Config.App.Id;

        Database.User = user;
        var info = await SystemService.GetSystemAsync(Database);
        user.AppName = info.AppName;
        user.CompName = info.CompName;
        if (!string.IsNullOrEmpty(user.OrgNo))
        {
            var orgName = await UserRepository.GetOrgNameAsync(Database, user.AppId, user.CompNo, user.OrgNo);
            if (string.IsNullOrEmpty(orgName))
                orgName = user.CompName;
            user.OrgName = orgName;
            if (string.IsNullOrEmpty(user.CompName))
                user.CompName = orgName;
        }

        SetUserAvatar(user);
    }

    private static void SetUserAvatar(UserInfo user)
    {
        if (user == null)
            return;

        user.AvatarUrl = user.Gender == "女" ? "img/face2.png" : "img/face1.png";
    }
}