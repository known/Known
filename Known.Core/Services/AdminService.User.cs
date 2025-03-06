namespace Known.Services;

partial class AdminService
{
    public async Task<PagingResult<UserInfo>> QueryUsersAsync(PagingCriteria criteria)
    {
        var db = Database;
        var sql = $@"select a.*,b.Name as Department 
from SysUser a 
left join SysOrganization b on b.Id=a.OrgNo 
where a.CompNo=@CompNo and a.UserName<>'admin'";
        var orgNoId = nameof(UserInfo.OrgNo);
        var orgNo = criteria.GetParameter<string>(orgNoId);
        if (!string.IsNullOrWhiteSpace(orgNo))
        {
            var org = await db.QueryByIdAsync<SysOrganization>(orgNo);
            if (org != null && org.Code != db.User?.CompNo)
                criteria.SetQuery(orgNoId, QueryType.Equal, orgNo);
            else
                criteria.RemoveQuery(orgNoId);
        }
        criteria.Fields[nameof(UserInfo.Name)] = "a.Name";
        return await db.QueryPageAsync<UserInfo>(sql, criteria);
    }

    public async Task<UserInfo> GetUserDataAsync(string id)
    {
        var database = Database;
        await database.OpenAsync();
        var user = await database.Query<SysUser>().Where(d => d.Id == id).FirstAsync<UserInfo>();
        user ??= new UserInfo();
        user.DefaultPassword = Config.System?.UserDefaultPwd;
        var roles = await database.Query<SysRole>().Where(d => d.Enabled).OrderBy(d => d.CreateTime).ToListAsync();
        var userRoles = await database.QueryListAsync<SysUserRole>(d => d.UserId == user.Id);
        var roleIds = userRoles?.Select(r => r.RoleId).ToList();
        user.Roles = roles.Select(r => new CodeInfo(r.Id, r.Name)).ToList();
        user.RoleIds = roleIds.ToArray();
        await database.CloseAsync();
        return user;
    }

    public async Task<Result> DeleteUsersAsync(List<UserInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var database = Database;
        var result = await UserHelper.OnDeletingAsync(database, infos);
        if (!result.IsValid)
            return result;

        return await database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in infos)
            {
                await db.DeleteAsync<SysUser>(item.Id);
                await db.DeleteAsync<SysUserRole>(d => d.UserId == item.Id);
                await UserHelper.OnDeletedAsync(db, item);
            }
        });
    }

    public async Task<Result> ChangeDepartmentAsync(List<UserInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var database = Database;
        var result = await UserHelper.OnChangingDepartmentAsync(database, infos);
        if (!result.IsValid)
            return result;

        return await database.TransactionAsync(Language.Save, async db =>
        {
            foreach (var item in infos)
            {
                var model = await db.QueryByIdAsync<SysUser>(item.Id);
                if (model != null)
                {
                    model.OrgNo = item.OrgNo;
                    await db.SaveAsync(model);
                    await UserHelper.OnChangedDepartmentAsync(db, model);
                }
            }
        });
    }

    public async Task<Result> EnableUsersAsync(List<UserInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var database = Database;
        var result = await UserHelper.OnEnablingAsync(database, infos);
        if (!result.IsValid)
            return result;

        return await database.TransactionAsync(Language.Enable, async db =>
        {
            foreach (var item in infos)
            {
                var model = await db.QueryByIdAsync<SysUser>(item.Id);
                if (model != null)
                {
                    model.Enabled = true;
                    await db.SaveAsync(model);
                    await UserHelper.OnEnabledAsync(db, model);
                }
            }
        });
    }

    public async Task<Result> DisableUsersAsync(List<UserInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var database = Database;
        var result = await UserHelper.OnDisablingAsync(database, infos);
        if (!result.IsValid)
            return result;

        return await database.TransactionAsync(Language.Disable, async db =>
        {
            foreach (var item in infos)
            {
                var model = await db.QueryByIdAsync<SysUser>(item.Id);
                if (model != null)
                {
                    model.Enabled = false;
                    await db.SaveAsync(model);
                    await UserHelper.OnDisabledAsync(db, model);
                }
            }
        });
    }

    public async Task<Result> SetUserPwdsAsync(List<UserInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var database = Database;
        var info = await database.GetSystemAsync();
        if (info == null || string.IsNullOrEmpty(info.UserDefaultPwd))
            return Result.Error(Language["Tip.NoDefaultPwd"]);

        return await database.TransactionAsync(Language.Reset, async db =>
        {
            foreach (var item in infos)
            {
                var model = await db.QueryByIdAsync<SysUser>(item.Id);
                if (model != null)
                {
                    model.Password = Utils.ToMd5(info.UserDefaultPwd);
                    await db.SaveAsync(model);
                }
            }
        });
    }

    public async Task<Result> SaveUserAsync(UserInfo info)
    {
        var database = Database;
        var model = await database.QueryByIdAsync<SysUser>(info.Id);
        model ??= new SysUser();
        model.FillModel(info);

        if (model.IsNew)
        {
            var sysInfo = await database.GetSystemAsync();
            if (sysInfo == null || string.IsNullOrEmpty(sysInfo.UserDefaultPwd))
                return Result.Error(Language["Tip.NoDefaultPwd"]);

            model.Password = Utils.ToMd5(sysInfo.UserDefaultPwd);
        }

        if (string.IsNullOrWhiteSpace(model.OrgNo))
            model.OrgNo = CurrentUser.OrgNo;
        var vr = model.Validate(Context);
        if (vr.IsValid)
        {
            model.UserName = model.UserName.ToLower();
            if (await database.ExistsAsync<SysUser>(d => d.Id != model.Id && d.UserName == model.UserName))
                vr.AddError(Language["Tip.UserNameExists"]);

            var result = await UserHelper.OnSavingAsync(database, info);
            if (!result.IsValid)
                vr.AddError(result.Message);
        }

        if (!vr.IsValid)
            return vr;

        return await database.TransactionAsync(Language.Save, async db =>
        {
            model.Role = string.Empty;
            await db.DeleteAsync<SysUserRole>(d => d.UserId == model.Id);
            var roles = await db.QueryListByIdAsync<SysRole>(info.RoleIds);
            if (roles != null && roles.Count > 0)
            {
                model.Role = string.Join(",", roles.Select(r => r.Name).ToArray());
                foreach (var item in roles)
                {
                    await db.InsertAsync(new SysUserRole { UserId = model.Id, RoleId = item.Id });
                }
            }
            await db.SaveAsync(model);
            await UserHelper.OnSavedAsync(db, model);
            info.Id = model.Id;
        }, info);
    }
}