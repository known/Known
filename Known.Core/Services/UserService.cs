using System.Collections.Concurrent;

namespace Known.Core.Services;

class UserService : BaseService
{
    internal UserService(Context context) : base(context) { }

    //User
    internal PagingResult<SysUser> QueryUsers(PagingCriteria criteria)
    {
        return UserRepository.QueryUsers(Database, criteria);
    }

    internal Result DeleteUsers(List<SysUser> entities)
    {
        if (entities == null || entities.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return Database.Transaction(Language.Delete, db =>
        {
            foreach (var item in entities)
            {
                db.Delete(item);
                UserRepository.DeleteUserRoles(db, item.Id);
            }
        });
    }

    internal Result EnableUsers(List<SysUser> entities)
    {
        if (entities == null || entities.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return Database.Transaction("启用", db =>
        {
            foreach (var item in entities)
            {
                item.Enabled = true;
                db.Save(item);
            }
        });
    }

    internal Result DisableUsers(List<SysUser> entities)
    {
        if (entities == null || entities.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return Database.Transaction("禁用", db =>
        {
            foreach (var item in entities)
            {
                item.Enabled = false;
                db.Save(item);
            }
        });
    }

    internal Result SetUserPwds(List<SysUser> entities)
    {
        if (entities == null || entities.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var info = SystemService.GetSystem(Database);
        if (info == null || string.IsNullOrEmpty(info.UserDefaultPwd))
            return Result.Error("用户默认密码未配置！");

        return Database.Transaction("重置", db =>
        {
            foreach (var item in entities)
            {
                item.Password = Utils.ToMd5(info.UserDefaultPwd);
                db.Save(item);
            }
        });
    }

    internal Result SaveUser(dynamic model)
    {
        List<SysRole> roles = null;
        var roleId = (string)model.RoleId;
        if (!string.IsNullOrWhiteSpace(roleId))
        {
            var roleIds = roleId.Split(',');
            roles = Database.QueryListById<SysRole>(roleIds);
        }
        var entity = Database.QueryById<SysUser>((string)model.Id);
        if (entity == null)
        {
            var user = CurrentUser;
            entity = new SysUser
            {
                OrgNo = user.OrgNo,
                FirstLoginTime = DateTime.Now,
                LastLoginTime = DateTime.Now,
                Enabled = true
            };

            var info = SystemService.GetSystem(Database);
            if (info == null || string.IsNullOrEmpty(info.UserDefaultPwd))
                return Result.Error("用户默认密码未配置！");

            if (Config.IsPlatform && user.IsTenant)
            {
                var tenant = SystemRepository.GetTenant(Database, user.CompNo);
                if (tenant == null)
                    return Result.Error("租户不存在！");

                var userCount = UserRepository.GetUserCount(Database);
                if (userCount >= tenant.UserCount)
                    return Result.Error("用户数已达上限，不能新增！");
            }

            var valid = PlatformHelper.CheckUser?.Invoke(Database, entity);
            if (valid != null && !valid.IsValid)
                return valid;

            entity.Password = Utils.ToMd5(info.UserDefaultPwd);
        }

        entity.FillModel(model);
        entity.Type = model.IsOperation == "True" ? Constants.UTOperation : "";
        var vr = entity.Validate();
        if (vr.IsValid)
        {
            entity.UserName = entity.UserName.ToLower();
            if (UserRepository.ExistsUserName(Database, entity.Id, entity.UserName))
            {
                vr.AddError("用户名已存在，请使用其他字符创建用户！");
            }
        }

        if (!vr.IsValid)
            return vr;

        return Database.Transaction(Language.Save, db =>
        {
            entity.Role = string.Empty;
            UserRepository.DeleteUserRoles(db, entity.Id);
            if (roles != null && roles.Count > 0)
            {
                entity.Role = string.Join(",", roles.Select(r => r.Name).ToArray());
                foreach (var item in roles)
                {
                    UserRepository.AddUserRole(db, entity.Id, item.Id);
                }
            }
            db.Save(entity);
            PlatformHelper.SetBizUser(db, entity);
        }, entity);
    }

    internal Result UpdateUser(dynamic model)
    {
        var user = CurrentUser;
        if (user == null)
            return Result.Error(Language.NoLogin);

        var entity = Database.QueryById<SysUser>(user.Id);
        if (entity == null)
            return Result.Error(Language.NoUser);

        entity.FillModel(model);
        var vr = entity.Validate();
        if (!vr.IsValid)
            return vr;

        Database.Save(entity);
        return Result.Success(Language.XXSuccess.Format(Language.Save), entity);
    }

    internal UserAuthInfo GetUserAuth(string userId)
    {
        var roles = RoleRepository.GetRoles(Database);
        var roleIds = UserRepository.GetUserRoles(Database, userId);
        var datas = PlatformHelper.UserDatas?.Invoke(Database);
        return new UserAuthInfo
        {
            Roles = roles.Select(r => new CodeInfo(r.Id, r.Name)).ToArray(),
            RoleIds = string.Join(",", roleIds),
            Datas = datas?.ToArray()
        };
    }

    //Account
    internal Result SignIn(LoginFormInfo info)
    {
        var userName = info.UserName.ToLower();
        var entity = UserRepository.GetUser(Database, userName, info.Password);
        if (entity == null)
            return Result.Error(Language.LoginNoNamePwd);

        if (!entity.Enabled)
            return Result.Error(Language.LoginDisabled);

        var isApp = Context.IsMobile;
        if (!entity.FirstLoginTime.HasValue)
        {
            entity.FirstLoginTime = DateTime.Now;
            entity.FirstLoginIP = info.IPAddress;
        }
        entity.LastLoginTime = DateTime.Now;
        entity.LastLoginIP = info.IPAddress;

        var user = Utils.MapTo<UserInfo>(entity);
        user.Token = Utils.GetGuid();
        SetUserInfo(user);
        cachedUsers[user.Token] = user;

        var type = Constants.LogTypeLogin;
        if (Context.IsMobile)
            type = "APP" + type;

        var database = Database;
        database.User = user;
        return database.Transaction(Language.Login, db =>
        {
            db.Save(entity);
            Logger.AddLog(db, type, $"{user.UserName}-{user.Name}", $"IP：{user.LastLoginIP}；所在地：{user.IPName}");
        }, user);
    }

    internal Result SignOut(string token)
    {
        var user = CurrentUser;
        if (string.IsNullOrWhiteSpace(token))
            token = user.Token;
        cachedUsers.TryRemove(token, out UserInfo _);

        var type = Constants.LogTypeLogout;
        if (Context.IsMobile)
            type = "APP" + type;
        Logger.AddLog(Database, type, $"{user.UserName}-{user.Name}", $"token: {token}");
        return Result.Success("退出成功！");
    }

    internal static ConcurrentDictionary<string, UserInfo> cachedUsers = new();

    internal static UserInfo GetUserByToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        return cachedUsers.Values.FirstOrDefault(u => u.Token == token);
    }

    internal static UserInfo GetUser(Database db, string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
            return null;

        userName = userName.Split('-')[0];
        var user = cachedUsers.Values.FirstOrDefault(u => u.UserName == userName);
        if (user != null)
            return user;

        return UserRepository.GetUser(db, userName);
    }

    internal AdminInfo GetAdmin()
    {
        var result = DictionaryService.RefreshCache(Database, CurrentUser);
        var admin = new AdminInfo
        {
            AppName = UserHelper.GetSystemName(Database),
            MessageCount = UserRepository.GetMessageCount(Database),
            UserSetting = UserHelper.GetUserSetting(Database),
            UserMenus = UserHelper.GetUserMenus(Database),
            Codes = result.Data as List<CodeInfo>
        };
        return admin;
    }

    internal Result UpdatePassword(PwdFormInfo info)
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

        var entity = UserRepository.GetUser(Database, user.UserName, info.OldPwd);
        if (entity == null)
            return Result.Error("当前密码不正确！");

        entity.Password = Utils.ToMd5(info.NewPwd);
        Database.Save(entity);
        return Result.Success(Language.XXSuccess.Format(Language.Update), entity.Id);
    }

    internal Result DeleteSetting(SettingFormInfo info)
    {
        var user = CurrentUser;
        if (user == null)
            return Result.Error(Language.NoLogin);

        var setting = SettingRepository.GetSettingByUser(Database, info.Type, info.Name);
        if (setting == null)
            return Result.Error(Language.NotExists.Format("设置"));

        Database.Delete(setting);
        return Result.Success(Language.DeleteSuccess);
    }

    internal Result SaveSetting(SettingFormInfo info)
    {
        var user = CurrentUser;
        if (user == null)
            return Result.Error(Language.NoLogin);

        var setting = SettingRepository.GetSettingByUser(Database, info.Type, info.Name);
        setting ??= new SysSetting { BizType = info.Type, BizName = info.Name };
        setting.BizData = info.Data;
        Database.Save(setting);
        return Result.Success(Language.SaveSuccess);
    }

    private void SetUserInfo(UserInfo user)
    {
        var sys = GetConfig<SystemInfo>(Database, SystemService.KeySystem);
        user.IsTenant = user.CompNo != sys.CompNo;
        user.IsGroupUser = user.OrgNo == user.CompNo;
        user.IsOperation = user.Type == Constants.UTOperation;
        user.AppName = Config.AppName;
        if (user.IsAdmin)
            user.AppId = Config.AppId;

        Database.User = user;
        var info = SystemService.GetSystem(Database);
        user.AppName = info.AppName;
        user.CompName = info.CompName;
        if (!string.IsNullOrEmpty(user.OrgNo))
        {
            var orgName = UserRepository.GetOrgName(Database, user.AppId, user.CompNo, user.OrgNo);
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

        user.AvatarUrl = user.Gender == "女" ? "/img/face2.png" : "/img/face1.png";
    }

    //Message
    internal PagingResult<SysMessage> QueryMessages(PagingCriteria criteria)
    {
        criteria.SetQuery(nameof(SysMessage.UserId), CurrentUser.UserName);
        return UserRepository.QueryMessages(Database, criteria);
    }

    internal Result DeleteMessages(List<SysMessage> entities)
    {
        if (entities == null || entities.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return Database.Transaction(Language.Delete, db =>
        {
            foreach (var item in entities)
            {
                db.Delete(item);
            }
        });
    }

    internal Result SaveMessage(dynamic model)
    {
        var entity = Database.QueryById<SysMessage>((string)model.Id);
        if (entity == null)
            return Result.Error("消息不存在！");

        entity.FillModel(model);
        var vr = entity.Validate();
        if (!vr.IsValid)
            return vr;

        var result = Database.Transaction(Language.Save, db =>
        {
            db.Save(entity);
        });
        result.Data = UserRepository.GetMessageCount(Database);
        return result;
    }
}