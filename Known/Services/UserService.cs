namespace Known.Services;

/// <summary>
/// 用户服务接口。
/// </summary>
public interface IUserService : IService
{
    /// <summary>
    /// 异步分页查询系统用户。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<UserInfo>> QueryUsersAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步分页查询用户信息。
    /// </summary>
    /// <param name="criteria">查询条件。</param>
    /// <returns></returns>
    Task<PagingResult<SysUser>> QueryUserDatasAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步获取用户信息。
    /// </summary>
    /// <param name="id">用户ID。</param>
    /// <returns></returns>
    Task<SysUser> GetUserDataAsync(string id);

    /// <summary>
    /// 异步删除用户。
    /// </summary>
    /// <param name="infos">用户列表。</param>
    /// <returns></returns>
    Task<Result> DeleteUsersAsync(List<SysUser> infos);

    /// <summary>
    /// 异步改变部门。
    /// </summary>
    /// <param name="infos">用户列表。</param>
    /// <returns></returns>
    Task<Result> ChangeDepartmentAsync(List<SysUser> infos);

    /// <summary>
    /// 异步启用用户。
    /// </summary>
    /// <param name="infos">用户列表。</param>
    /// <returns></returns>
    Task<Result> EnableUsersAsync(List<SysUser> infos);

    /// <summary>
    /// 异步禁用用户。
    /// </summary>
    /// <param name="infos">用户列表。</param>
    /// <returns></returns>
    Task<Result> DisableUsersAsync(List<SysUser> infos);

    /// <summary>
    /// 异步重置密码。
    /// </summary>
    /// <param name="infos">用户列表。</param>
    /// <returns></returns>
    Task<Result> SetUserPwdsAsync(List<SysUser> infos);

    /// <summary>
    /// 异步保存用户。
    /// </summary>
    /// <param name="info">用户信息。</param>
    /// <returns></returns>
    Task<Result> SaveUserAsync(SysUser info);
}

[Client]
class UserClient(HttpClient http) : ClientBase(http), IUserService
{
    public Task<PagingResult<UserInfo>> QueryUsersAsync(PagingCriteria criteria) => Http.QueryAsync<UserInfo>("/User/QueryUsers", criteria);
    public Task<PagingResult<SysUser>> QueryUserDatasAsync(PagingCriteria criteria) => Http.QueryAsync<SysUser>("/User/QueryUserDatas", criteria);
    public Task<SysUser> GetUserDataAsync(string id) => Http.GetAsync<SysUser>($"/User/GetUserData?id={id}");
    public Task<Result> DeleteUsersAsync(List<SysUser> infos) => Http.PostAsync("/User/DeleteUsers", infos);
    public Task<Result> ChangeDepartmentAsync(List<SysUser> infos) => Http.PostAsync("/User/ChangeDepartment", infos);
    public Task<Result> EnableUsersAsync(List<SysUser> infos) => Http.PostAsync("/User/EnableUsers", infos);
    public Task<Result> DisableUsersAsync(List<SysUser> infos) => Http.PostAsync("/User/DisableUsers", infos);
    public Task<Result> SetUserPwdsAsync(List<SysUser> infos) => Http.PostAsync("/User/SetUserPwds", infos);
    public Task<Result> SaveUserAsync(SysUser info) => Http.PostAsync("/User/SaveUser", info);
}

[WebApi, Service]
class UserService(Context context, IUserHandler handler) : ServiceBase(context), IUserService
{
    public async Task<PagingResult<UserInfo>> QueryUsersAsync(PagingCriteria criteria)
    {
        return await QueryUsersAsync<UserInfo>(criteria);
    }

    public async Task<PagingResult<SysUser>> QueryUserDatasAsync(PagingCriteria criteria)
    {
        return await QueryUsersAsync<SysUser>(criteria);
    }

    public async Task<SysUser> GetUserDataAsync(string id)
    {
        SysUser user = null;
        await Database.QueryActionAsync(async db =>
        {
            var sys = await db.GetSystemAsync();
            user = await db.QueryByIdAsync<SysUser>(id);
            user ??= new SysUser { Password = sys?.UserDefaultPwd };
            user.DefaultPassword = sys?.UserDefaultPwd;
            var roles = await db.Query<SysRole>().Where(d => d.Enabled).OrderBy(d => d.CreateTime).ToListAsync();
            var userRoles = await db.QueryListAsync<SysUserRole>(d => d.UserId == user.Id);
            var roleIds = userRoles?.Select(r => r.RoleId).ToList();
            user.Roles = [.. roles.Select(r => new CodeInfo(r.Id, r.Name))];
            user.RoleIds = [.. roleIds];
        });
        return user;
    }

    public async Task<Result> DeleteUsersAsync(List<SysUser> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        if (infos.Exists(d => d.UserName == CurrentUser.UserName))
            return Result.Error(Language.TipNotDeleteSelf);

        var database = Database;
        var result = await handler.OnDeletingAsync(database, infos);
        if (!result.IsValid)
            return result;

        return await database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in infos)
            {
                await db.DeleteAsync<SysUser>(item.Id);
                await db.DeleteAsync<SysUserRole>(d => d.UserId == item.Id);
                await handler.OnDeletedAsync(db, item);
            }
        });
    }

    public async Task<Result> ChangeDepartmentAsync(List<SysUser> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var database = Database;
        var result = await handler.OnChangingDepartmentAsync(database, infos);
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
                    await handler.OnChangedDepartmentAsync(db, model);
                }
            }
        });
    }

    public async Task<Result> EnableUsersAsync(List<SysUser> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var database = Database;
        var result = await handler.OnEnablingAsync(database, infos);
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
                    await handler.OnEnabledAsync(db, model);
                }
            }
        });
    }

    public async Task<Result> DisableUsersAsync(List<SysUser> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var database = Database;
        var result = await handler.OnDisablingAsync(database, infos);
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
                    await handler.OnDisabledAsync(db, model);
                }
            }
        });
    }

    public async Task<Result> SetUserPwdsAsync(List<SysUser> infos)
    {
        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var database = Database;
        var info = await database.GetSystemAsync();
        if (info == null || string.IsNullOrEmpty(info.UserDefaultPwd))
            return Result.Error(Language.TipNoDefaultPwd);

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

    public async Task<Result> SaveUserAsync(SysUser info)
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

            var result = await handler.OnSavingAsync(database, info);
            if (!result.IsValid)
                vr.AddError(result.Message);
        }

        if (!vr.IsValid)
            return vr;

        return await database.TransactionAsync(Language.Save, async db =>
        {
            if (model.IsNew)
                await db.SetNewUserAsync(model);
            model.Role = string.Empty;
            await db.DeleteAsync<SysUserRole>(d => d.UserId == model.Id);
            var roles = await db.QueryListByIdAsync<SysRole>(info.RoleIds);
            if (roles != null && roles.Count > 0)
            {
                model.Role = string.Join(",", [.. roles.Select(r => r.Name)]);
                foreach (var item in roles)
                {
                    await db.InsertAsync(new SysUserRole { UserId = model.Id, RoleId = item.Id });
                }
            }
            await db.SaveAsync(model);
            await handler.OnSavedAsync(db, model);
            info.Id = model.Id;
        }, info);
    }

    private async Task<PagingResult<T>> QueryUsersAsync<T>(PagingCriteria criteria) where T : class, new()
    {
        var db = Database;
        var sql = "select * from SysUser a";
        var orgNoId = nameof(SysUser.OrgNo);
        var orgNo = criteria.GetParameter<string>(orgNoId);
        if (!string.IsNullOrWhiteSpace(orgNo))
        {
            sql = "select a.*,b.Name as Department from SysUser a left join SysOrganization b on b.Id=a.OrgNo";
            criteria.RemoveQuery(orgNoId);
        }
        sql += " where a.CompNo=@CompNo and a.UserName<>'admin'";
        criteria.RemoveQuery("Key");
        var key = criteria.GetParameter<string>("Key");
        if (!string.IsNullOrWhiteSpace(key))
        {
            sql += " and (a.UserName like @Key or a.Name like @Key)";
            criteria.SetQuery("Key", QueryType.Contain, $"%{key}%");
        }
        if (!string.IsNullOrWhiteSpace(orgNo))
        {
            var org = await db.QueryByIdAsync<SysOrganization>(orgNo);
            if (org != null && org.Code != db.User?.CompNo)
                criteria.SetQuery(orgNoId, QueryType.Equal, orgNo);
            else
                criteria.RemoveQuery(orgNoId);
        }
        criteria.Fields[nameof(SysUser.Name)] = "a.Name";
        return await db.QueryPageAsync<T>(sql, criteria);
    }
}