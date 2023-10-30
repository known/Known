namespace Known.Services;

class UserService : BaseService
{
    //User
    public Task<PagingResult<SysUser>> QueryUsersAsync(PagingCriteria criteria)
    {
        return UserRepository.QueryUsersAsync(Database, criteria);
    }

    public async Task<Result> DeleteUsersAsync(List<SysUser> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in models)
            {
                await db.DeleteAsync(item);
                await UserRepository.DeleteUserRolesAsync(db, item.Id);
            }
        });
    }

    public async Task<Result> ChangeDepartmentAsync(List<SysUser> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Save, async db =>
        {
            foreach (var item in models)
            {
                await db.SaveAsync(item);
            }
        });
    }

    public async Task<Result> EnableUsersAsync(List<SysUser> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync("启用", async db =>
        {
            foreach (var item in models)
            {
                item.Enabled = true;
                await db.SaveAsync(item);
            }
        });
    }

    public async Task<Result> DisableUsersAsync(List<SysUser> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync("禁用", async db =>
        {
            foreach (var item in models)
            {
                item.Enabled = false;
                await db.SaveAsync(item);
            }
        });
    }

    public async Task<Result> SetUserPwdsAsync(List<SysUser> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var info = await SystemService.GetSystemAsync(Database);
        if (info == null || string.IsNullOrEmpty(info.UserDefaultPwd))
            return Result.Error("用户默认密码未配置！");

        return await Database.TransactionAsync("重置", async db =>
        {
            foreach (var item in models)
            {
                item.Password = Utils.ToMd5(info.UserDefaultPwd);
                await db.SaveAsync(item);
            }
        });
    }

    public async Task<Result> SaveUserAsync(dynamic model)
    {
        List<SysRole> roles = null;
        var roleId = (string)model.RoleId;
        if (!string.IsNullOrWhiteSpace(roleId))
        {
            var roleIds = roleId.Split(',');
            roles = await Database.QueryListByIdAsync<SysRole>(roleIds);
        }
        var user = CurrentUser;
        var entity = await Database.QueryByIdAsync<SysUser>((string)model.Id);
        if (entity == null)
        {
            entity = new SysUser
            {
                OrgNo = user.OrgNo,
                FirstLoginTime = DateTime.Now,
                LastLoginTime = DateTime.Now,
                Enabled = true
            };

            var info = await SystemService.GetSystemAsync(Database);
            if (info == null || string.IsNullOrEmpty(info.UserDefaultPwd))
                return Result.Error("用户默认密码未配置！");

            if (Config.IsPlatform && user.IsTenant)
            {
                //var tenant = SystemRepository.GetTenant(Database, user.CompNo);
                //if (tenant == null)
                //    return Result.Error("租户不存在！");

                //var userCount = UserRepository.GetUserCount(Database);
                //if (userCount >= tenant.UserCount)
                //    return Result.Error("用户数已达上限，不能新增！");
            }

            var valid = PlatformHelper.CheckUser?.Invoke(Database, entity);
            if (valid != null && !valid.IsValid)
                return valid;

            entity.Password = Utils.ToMd5(info.UserDefaultPwd);
        }

        entity.FillModel(model);
        if (string.IsNullOrWhiteSpace(entity.OrgNo))
            entity.OrgNo = user.OrgNo;
        entity.Type = model.IsOperation == "True" ? Constants.UTOperation : "";
        var vr = entity.Validate();
        if (vr.IsValid)
        {
            entity.UserName = entity.UserName.ToLower();
            if (await UserRepository.ExistsUserNameAsync(Database, entity.Id, entity.UserName))
            {
                vr.AddError("用户名已存在，请使用其他字符创建用户！");
            }
        }

        if (!vr.IsValid)
            return vr;

        return await Database.TransactionAsync(Language.Save, async db =>
        {
            entity.Role = string.Empty;
            await UserRepository.DeleteUserRolesAsync(db, entity.Id);
            if (roles != null && roles.Count > 0)
            {
                entity.Role = string.Join(",", roles.Select(r => r.Name).ToArray());
                foreach (var item in roles)
                {
                    await UserRepository.AddUserRoleAsync(db, entity.Id, item.Id);
                }
            }
            await db.SaveAsync(entity);
            PlatformHelper.SetBizUser(db, entity);
        }, entity);
    }

    public async Task<Result> UpdateUserAsync(dynamic model)
    {
        var user = CurrentUser;
        if (user == null)
            return Result.Error(Language.NoLogin);

        var entity = await Database.QueryByIdAsync<SysUser>(user.Id);
        if (entity == null)
            return Result.Error(Language.NoUser);

        entity.FillModel(model);
        var vr = entity.Validate();
        if (!vr.IsValid)
            return vr;

        await Database.SaveAsync(entity);
        return Result.Success(Language.XXSuccess.Format(Language.Save), entity);
    }

    public async Task<UserAuthInfo> GetUserAuthAsync(string userId)
    {
        var roles = await RoleRepository.GetRolesAsync(Database);
        var roleIds = await UserRepository.GetUserRolesAsync(Database, userId);
        var datas = PlatformHelper.UserDatas?.Invoke(Database);
        return new UserAuthInfo
        {
            Roles = roles.Select(r => new CodeInfo(r.Id, r.Name)).ToArray(),
            RoleIds = string.Join(",", roleIds),
            Datas = datas?.ToArray()
        };
    }

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

        var type = Constants.LogTypeLogin;
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
        cachedUsers.TryRemove(token, out UserInfo _);

        await Logger.AddLogAsync(Database, Constants.LogTypeLogout, $"{user.UserName}-{user.Name}", $"token: {token}");
        return Result.Success("退出成功！");
    }

    internal static ConcurrentDictionary<string, UserInfo> cachedUsers = new();

    internal static UserInfo GetUserByToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return null;

        return cachedUsers.Values.FirstOrDefault(u => u.Token == token);
    }

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
        var result = await DictionaryService.RefreshCacheAsync(Database, CurrentUser);
        var admin = new AdminInfo
        {
            AppName = await UserHelper.GetSystemNameAsync(Database),
            MessageCount = await UserRepository.GetMessageCountAsync(Database),
            UserSetting = await UserHelper.GetUserSettingAsync(Database),
            UserMenus = await UserHelper.GetUserMenusAsync(Database),
            Codes = result.Data as List<CodeInfo>
        };
        return admin;
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

    public async Task<Result> DeleteSettingAsync(SettingFormInfo info)
    {
        var user = CurrentUser;
        if (user == null)
            return Result.Error(Language.NoLogin);

        var setting = await SettingRepository.GetSettingByUserAsync(Database, info.Type, info.Name);
        if (setting == null)
            return Result.Error(Language.NotExists.Format("设置"));

        await Database.DeleteAsync(setting);
        return Result.Success(Language.DeleteSuccess);
    }

    public async Task<Result> SaveSettingAsync(SettingFormInfo info)
    {
        var user = CurrentUser;
        if (user == null)
            return Result.Error(Language.NoLogin);

        var setting = await SettingRepository.GetSettingByUserAsync(Database, info.Type, info.Name);
        setting ??= new SysSetting { BizType = info.Type, BizName = info.Name };
        setting.BizData = info.Data;
        await Database.SaveAsync(setting);
        return Result.Success(Language.SaveSuccess);
    }

    private async Task SetUserInfoAsync(UserInfo user)
    {
        var sys = await GetConfigAsync<SystemInfo>(Database, SystemService.KeySystem);
        user.IsTenant = user.CompNo != sys.CompNo;
        user.AppName = Config.AppName;
        if (user.IsAdmin)
            user.AppId = Config.AppId;

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

    //Message
    public Task<PagingResult<SysMessage>> QueryMessagesAsync(PagingCriteria criteria)
    {
        criteria.SetQuery(nameof(SysMessage.UserId), CurrentUser.UserName);
        return UserRepository.QueryMessagesAsync(Database, criteria);
    }

    public async Task<Result> DeleteMessagesAsync(List<SysMessage> entities)
    {
        if (entities == null || entities.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return await Database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in entities)
            {
                await db.DeleteAsync(item);
            }
        });
    }

    public async Task<Result> SaveMessageAsync(dynamic model)
    {
        var entity = await Database.QueryByIdAsync<SysMessage>((string)model.Id);
        if (entity == null)
            return Result.Error("消息不存在！");

        entity.FillModel(model);
        var vr = entity.Validate();
        if (!vr.IsValid)
            return vr;

        var result = await Database.TransactionAsync(Language.Save, async db =>
        {
            await db.SaveAsync(entity);
        });
        result.Data = UserRepository.GetMessageCountAsync(Database);
        return result;
    }
}