namespace Known.Services;

public interface IUserService : IService
{
    Task<PagingResult<SysUser>> QueryUsersAsync(PagingCriteria criteria);
    Task<SysUser> GetUserAsync(string id);
    Task<SysUser> GetUserDataAsync(string id);
    Task<Result> DeleteUsersAsync(List<SysUser> models);
    Task<Result> ChangeDepartmentAsync(List<SysUser> models);
    Task<Result> EnableUsersAsync(List<SysUser> models);
    Task<Result> DisableUsersAsync(List<SysUser> models);
    Task<Result> SetUserPwdsAsync(List<SysUser> models);
    Task<Result> UpdateUserAsync(SysUser model);
    Task<Result> SaveUserAsync(SysUser model);
}

class UserService(Context context) : ServiceBase(context), IUserService
{
    //User
    public Task<PagingResult<SysUser>> QueryUsersAsync(PagingCriteria criteria)
    {
        return DataRepository.QueryUsersAsync(Database, criteria);
    }

    public Task<SysUser> GetUserAsync(string id) => Database.QueryByIdAsync<SysUser>(id);

    public async Task<SysUser> GetUserDataAsync(string id)
    {
        var db = Database;
        await db.OpenAsync();
        var user = await db.QueryByIdAsync<SysUser>(id);
        user ??= new SysUser();
        var roles = await db.Query<SysRole>().Where(d => d.CompNo == db.User.CompNo && d.Enabled)
                            .OrderBy(d => d.CreateTime).ToListAsync();
        var userRoles = await db.QueryListAsync<SysUserRole>(d => d.UserId == user.Id);
        var roleIds = userRoles?.Select(r => r.RoleId).ToList();
        //var datas = PlatformHelper.UserDatas?.Invoke(db);
        user.Roles = roles.Select(r => new CodeInfo(r.Id, r.Name)).ToList();
        user.RoleIds = roleIds.ToArray();
        //user.Datas = datas?.ToArray();
        await db.CloseAsync();
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
                await db.DeleteAsync<SysUserRole>(d => d.UserId == item.Id);
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

        return await Database.TransactionAsync(Language.Enable, async db =>
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

        return await Database.TransactionAsync(Language.Disable, async db =>
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
            return Result.Error(Language["Tip.NoDefaultPwd"]);

        return await Database.TransactionAsync(Language.Reset, async db =>
        {
            foreach (var item in models)
            {
                item.Password = Utils.ToMd5(info.UserDefaultPwd);
                await db.SaveAsync(item);
            }
        });
    }

    public async Task<Result> UpdateUserAsync(SysUser model)
    {
        if (model == null)
            return Result.Error(Language["Tip.NoUser"]);

        var vr = model.Validate(Context);
        if (!vr.IsValid)
            return vr;

        await Database.SaveAsync(model);
        return Result.Success(Language.Success(Language.Save), model);
    }

    public async Task<Result> SaveUserAsync(SysUser model)
    {
        List<SysRole> roles = null;
        if (model.RoleIds != null && model.RoleIds.Length > 0)
            roles = await Database.QueryListByIdAsync<SysRole>(model.RoleIds);
        var user = CurrentUser;
        if (model.IsNew)
        {
            var info = await SystemService.GetSystemAsync(Database);
            if (info == null || string.IsNullOrEmpty(info.UserDefaultPwd))
                return Result.Error(Language["Tip.NoDefaultPwd"]);

            model.Password = Utils.ToMd5(info.UserDefaultPwd);
        }

        if (string.IsNullOrWhiteSpace(model.OrgNo))
            model.OrgNo = user.OrgNo;
        var vr = model.Validate(Context);
        if (vr.IsValid)
        {
            model.UserName = model.UserName.ToLower();
            if (await Database.ExistsAsync<SysUser>(d => d.Id != model.Id && d.UserName == model.UserName))
            {
                vr.AddError(Language["Tip.UserNameExists"]);
            }
        }

        if (!vr.IsValid)
            return vr;

        return await Database.TransactionAsync(Language.Save, async db =>
        {
            model.Role = string.Empty;
            await db.DeleteAsync<SysUserRole>(d => d.UserId == model.Id);
            if (roles != null && roles.Count > 0)
            {
                model.Role = string.Join(",", roles.Select(r => r.Name).ToArray());
                foreach (var item in roles)
                {
                    await db.InsertDataAsync(new SysUserRole { UserId = model.Id, RoleId = item.Id });
                }
            }
            await db.SaveAsync(model);
            //PlatformHelper.SetBizUser(db, model);
        }, model);
    }

    internal static async Task SyncUserAsync(Database db, SysUser user)
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
            var info = await SystemService.GetSystemAsync(db);
            if (!string.IsNullOrWhiteSpace(user.Password))
                model.Password = Utils.ToMd5(user.Password);
            else if (info != null)
                model.Password = Utils.ToMd5(info.UserDefaultPwd);
            await db.SaveAsync(model);
            var role = await db.QueryAsync<SysRole>(d => d.CompNo == user.CompNo && d.Name == user.Role);
            if (role != null)
                await db.InsertDataAsync(new SysUserRole { UserId = model.Id, RoleId = role.Id });
        }
    }

    //Setting
    //public async Task<Result> DeleteSettingAsync(SettingFormInfo info)
    //{
    //    var user = CurrentUser;
    //    if (user == null)
    //        return Result.Error(Language["Tip.NoLogin"]);

    //    var setting = await SettingRepository.GetSettingByUserAsync(Database, info.Type, info.Name);
    //    if (setting == null)
    //        return Result.Error(Language.NotExists.Format("设置"));

    //    await Database.DeleteAsync(setting);
    //    return Result.Success(Language.DeleteSuccess);
    //}

    //public async Task<Result> SaveSettingAsync(SettingFormInfo info)
    //{
    //    var user = CurrentUser;
    //    if (user == null)
    //        return Result.Error(Language["Tip.NoLogin"]);

    //    var setting = await SettingRepository.GetSettingByUserAsync(Database, info.Type, info.Name);
    //    setting ??= new SysSetting { BizType = info.Type, BizName = info.Name };
    //    setting.BizData = info.Data;
    //    await Database.SaveAsync(setting);
    //    return Result.Success(Language.SaveSuccess);
    //}

    //Message
    //public Task<PagingResult<SysMessage>> QueryMessagesAsync(PagingCriteria criteria)
    //{
    //    criteria.SetQuery(nameof(SysMessage.UserId), CurrentUser.UserName);
    //    return UserRepository.QueryMessagesAsync(Database, criteria);
    //}

    //public async Task<Result> DeleteMessagesAsync(List<SysMessage> entities)
    //{
    //    if (entities == null || entities.Count == 0)
    //        return Result.Error(Language.SelectOneAtLeast);

    //    return await Database.TransactionAsync(Language.Delete, async db =>
    //    {
    //        foreach (var item in entities)
    //        {
    //            await db.DeleteAsync(item);
    //        }
    //    });
    //}

    //public async Task<Result> SaveMessageAsync(SysMessage model)
    //{
    //    var vr = model.Validate(Context);
    //    if (!vr.IsValid)
    //        return vr;

    //    var result = await Database.TransactionAsync(Language.Save, async db =>
    //    {
    //        await db.SaveAsync(model);
    //    });
    //    result.Data = UserRepository.GetMessageCountAsync(Database);
    //    return result;
    //}
}