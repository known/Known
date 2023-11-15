using Known.Entities;
using Known.Repositories;

namespace Known.Services;

class UserService : BaseService
{
    //User
    public Task<PagingResult<SysUser>> QueryUsersAsync(PagingCriteria criteria)
    {
        return UserRepository.QueryUsersAsync(Database, criteria);
    }

    public Task<SysUser> GetUserAsync(string id) => Database.QueryByIdAsync<SysUser>(id);

    public async Task<SysUser> GetUserAsync(SysUser user)
    {
        var roles = await RoleRepository.GetRolesAsync(Database);
        var roleIds = await UserRepository.GetUserRolesAsync(Database, user.Id);
        //var datas = PlatformHelper.UserDatas?.Invoke(Database);
        user.Roles = roles.Select(r => new CodeInfo(r.Id, r.Name)).ToList();
        user.RoleIds = roleIds.ToArray();
        //user.Datas = datas?.ToArray();
        return user;
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

            if (Config.App.IsPlatform && user.IsTenant)
            {
                //var tenant = SystemRepository.GetTenant(Database, user.CompNo);
                //if (tenant == null)
                //    return Result.Error("租户不存在！");

                //var userCount = UserRepository.GetUserCount(Database);
                //if (userCount >= tenant.UserCount)
                //    return Result.Error("用户数已达上限，不能新增！");
            }

            //var valid = PlatformHelper.CheckUser?.Invoke(Database, entity);
            //if (valid != null && !valid.IsValid)
            //    return valid;

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
            //PlatformHelper.SetBizUser(db, entity);
        }, entity);
    }

    //Setting
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