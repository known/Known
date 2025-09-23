namespace Known.Services;

public interface IUserService : IService
{
    Task<PagingResult<UserDataInfo>> QueryUserDatasAsync(PagingCriteria criteria);
    Task<UserDataInfo> GetUserDataAsync(string id);
    Task<Result> DeleteUsersAsync(List<UserDataInfo> infos);
    Task<Result> ChangeDepartmentAsync(List<UserDataInfo> infos);
    Task<Result> EnableUsersAsync(List<UserDataInfo> infos);
    Task<Result> DisableUsersAsync(List<UserDataInfo> infos);
    Task<Result> SetUserPwdsAsync(List<UserDataInfo> infos);
    Task<Result> SaveUserAsync(UserDataInfo info);
}

[Client]
class UserClient(HttpClient http) : ClientBase(http), IUserService
{
    public Task<PagingResult<UserDataInfo>> QueryUserDatasAsync(PagingCriteria criteria) => Http.QueryAsync<UserDataInfo>("/User/QueryUserDatas", criteria);
    public Task<UserDataInfo> GetUserDataAsync(string id) => Http.GetAsync<UserDataInfo>($"/User/GetUserData?id={id}");
    public Task<Result> DeleteUsersAsync(List<UserDataInfo> infos) => Http.PostAsync("/User/DeleteUsers", infos);
    public Task<Result> ChangeDepartmentAsync(List<UserDataInfo> infos) => Http.PostAsync("/User/ChangeDepartment", infos);
    public Task<Result> EnableUsersAsync(List<UserDataInfo> infos) => Http.PostAsync("/User/EnableUsers", infos);
    public Task<Result> DisableUsersAsync(List<UserDataInfo> infos) => Http.PostAsync("/User/DisableUsers", infos);
    public Task<Result> SetUserPwdsAsync(List<UserDataInfo> infos) => Http.PostAsync("/User/SetUserPwds", infos);
    public Task<Result> SaveUserAsync(UserDataInfo info) => Http.PostAsync("/User/SaveUser", info);
}

[WebApi, Service]
class UserService(Context context) : ServiceBase(context), IUserService
{
    public async Task<PagingResult<UserDataInfo>> QueryUserDatasAsync(PagingCriteria criteria)
    {
        return await QueryUsersAsync<UserDataInfo>(criteria);
    }

    public async Task<UserDataInfo> GetUserDataAsync(string id)
    {
        UserDataInfo user = null;
        await Database.QueryActionAsync(async db =>
        {
            var sys = await db.GetSystemAsync();
            user = await db.Query<SysUser>().Where(d => d.Id == id).FirstAsync<UserDataInfo>();
            user ??= new UserDataInfo { Password = sys?.UserDefaultPwd };
            user.DefaultPassword = sys?.UserDefaultPwd;
            var roles = await db.Query<SysRole>().Where(d => d.Enabled).OrderBy(d => d.CreateTime).ToListAsync();
            var userRoles = await db.QueryListAsync<SysUserRole>(d => d.UserId == user.Id);
            var roleIds = userRoles?.Select(r => r.RoleId).ToList();
            user.Roles = [.. roles.Select(r => new CodeInfo(r.Id, r.Name))];
            user.RoleIds = [.. roleIds];
        });
        return user;
    }

    public async Task<Result> DeleteUsersAsync(List<UserDataInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        if (infos.Exists(d => d.UserName == CurrentUser.UserName))
            return Result.Error(AdminLanguage.TipNotDeleteSelf);

        var database = Database;
        var result = await UserHelper.OnDeletingAsync(database, [.. infos.Select(u => (UserInfo)u)]);
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

    public async Task<Result> ChangeDepartmentAsync(List<UserDataInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var database = Database;
        var result = await UserHelper.OnChangingDepartmentAsync(database, [.. infos.Select(u => (UserInfo)u)]);
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

    public async Task<Result> EnableUsersAsync(List<UserDataInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var database = Database;
        var result = await UserHelper.OnEnablingAsync(database, [.. infos.Select(u => (UserInfo)u)]);
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

    public async Task<Result> DisableUsersAsync(List<UserDataInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var database = Database;
        var result = await UserHelper.OnDisablingAsync(database, [.. infos.Select(u => (UserInfo)u)]);
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

    public async Task<Result> SetUserPwdsAsync(List<UserDataInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var database = Database;
        var info = await database.GetSystemAsync();
        if (info == null || string.IsNullOrEmpty(info.UserDefaultPwd))
            return Result.Error(AdminLanguage.TipNoDefaultPwd);

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

    public async Task<Result> SaveUserAsync(UserDataInfo info)
    {
        var database = Database;
        var model = await database.QueryByIdAsync<SysUser>(info.Id);
        var orgPassword = model?.Password;
        model ??= new SysUser();
        model.FillModel(info);

        // 与原始密码不一致，更新密码
        if (model.Password != orgPassword)
            model.Password = Utils.ToMd5(model.Password);

        if (string.IsNullOrWhiteSpace(model.OrgNo))
            model.OrgNo = CurrentUser.OrgNo;
        var vr = model.Validate(Context);
        if (vr.IsValid)
        {
            model.UserName = model.UserName.ToLower();
            if (await database.ExistsAsync<SysUser>(d => d.Id != model.Id && d.UserName == model.UserName))
                vr.AddError(Language.TipUserNameExists);

            var result = await UserHelper.OnSavingAsync(database, info);
            if (!result.IsValid)
                vr.AddError(result.Message);
        }

        if (!vr.IsValid)
            return vr;

        return await database.TransactionAsync(Language.Save, async db =>
        {
            if (model.IsNew)
                CoreConfig.OnNewUser?.Invoke(db, model);
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

    private async Task<PagingResult<T>> QueryUsersAsync<T>(PagingCriteria criteria) where T : class, new()
    {
        var db = Database;
        var sql = $@"select a.*,b.Name as Department 
from SysUser a 
left join SysOrganization b on b.Id=a.OrgNo 
where a.CompNo=@CompNo and a.UserName<>'admin'";
        var orgNoId = nameof(UserDataInfo.OrgNo);
        var orgNo = criteria.GetParameter<string>(orgNoId);
        if (!string.IsNullOrWhiteSpace(orgNo))
        {
            var org = await db.QueryByIdAsync<SysOrganization>(orgNo);
            if (org != null && org.Code != db.User?.CompNo)
                criteria.SetQuery(orgNoId, QueryType.Equal, orgNo);
            else
                criteria.RemoveQuery(orgNoId);
        }
        criteria.Fields[nameof(UserDataInfo.Name)] = "a.Name";
        return await db.QueryPageAsync<T>(sql, criteria);
    }
}