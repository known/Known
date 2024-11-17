namespace Known.Services;

/// <summary>
/// 系统用户服务接口。
/// </summary>
public interface IUserService : IService
{
    /// <summary>
    /// 异步分页查询系统用户。
    /// </summary>
    /// <param name="criteria">查询条件对象。</param>
    /// <returns>分页结果。</returns>
    Task<PagingResult<SysUser>> QueryUsersAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步获取系统用户数据。
    /// </summary>
    /// <param name="id">用户ID。</param>
    /// <returns>系统用户数据。</returns>
    Task<SysUser> GetUserDataAsync(string id);

    /// <summary>
    /// 异步删除系统用户。
    /// </summary>
    /// <param name="models">系统用户列表。</param>
    /// <returns>删除结果。</returns>
    Task<Result> DeleteUsersAsync(List<SysUser> models);

    /// <summary>
    /// 异步切换系统用户所属部门。
    /// </summary>
    /// <param name="models">系统用户列表。</param>
    /// <returns>切换结果。</returns>
    Task<Result> ChangeDepartmentAsync(List<SysUser> models);

    /// <summary>
    /// 异步启用系统用户。
    /// </summary>
    /// <param name="models">系统用户列表。</param>
    /// <returns>启用结果。</returns>
    Task<Result> EnableUsersAsync(List<SysUser> models);

    /// <summary>
    /// 异步禁用系统用户。
    /// </summary>
    /// <param name="models">系统用户列表。</param>
    /// <returns>禁用结果。</returns>
    Task<Result> DisableUsersAsync(List<SysUser> models);

    /// <summary>
    /// 异步重置系统用户密码。
    /// </summary>
    /// <param name="models">系统用户列表。</param>
    /// <returns>重置结果。</returns>
    Task<Result> SetUserPwdsAsync(List<SysUser> models);

    /// <summary>
    /// 异步保存系统用户。
    /// </summary>
    /// <param name="model">系统用户信息。</param>
    /// <returns>保存结果。</returns>
    Task<Result> SaveUserAsync(SysUser model);
}

class UserService(Context context) : ServiceBase(context), IUserService
{
    public async Task<PagingResult<SysUser>> QueryUsersAsync(PagingCriteria criteria)
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
        return await db.QueryPageAsync<SysUser>(sql, criteria);
    }

    public async Task<SysUser> GetUserDataAsync(string id)
    {
        var database = Database;
        await database.OpenAsync();
        var user = await database.QueryByIdAsync<SysUser>(id);
        user ??= new SysUser();
        var roles = await database.Query<SysRole>().Where(d => d.Enabled).OrderBy(d => d.CreateTime).ToListAsync();
        var userRoles = await database.QueryListAsync<SysUserRole>(d => d.UserId == user.Id);
        var roleIds = userRoles?.Select(r => r.RoleId).ToList();
        //var datas = PlatformHelper.UserDatas?.Invoke(db);
        user.Roles = roles.Select(r => new CodeInfo(r.Id, r.Name)).ToList();
        user.RoleIds = roleIds.ToArray();
        //user.Datas = datas?.ToArray();
        await database.CloseAsync();
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

        var database = Database;
        var info = await database.GetSystemAsync();
        if (info == null || string.IsNullOrEmpty(info.UserDefaultPwd))
            return Result.Error(Language["Tip.NoDefaultPwd"]);

        return await database.TransactionAsync(Language.Reset, async db =>
        {
            foreach (var item in models)
            {
                item.Password = Utils.ToMd5(info.UserDefaultPwd);
                await db.SaveAsync(item);
            }
        });
    }

    public async Task<Result> SaveUserAsync(SysUser model)
    {
        List<SysRole> roles = null;
        var database = Database;
        if (model.RoleIds != null && model.RoleIds.Length > 0)
            roles = await database.QueryListByIdAsync<SysRole>(model.RoleIds);
        var user = CurrentUser;
        if (model.IsNew)
        {
            var info = await database.GetSystemAsync();
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
            if (await database.ExistsAsync<SysUser>(d => d.Id != model.Id && d.UserName == model.UserName))
            {
                vr.AddError(Language["Tip.UserNameExists"]);
            }
        }

        if (!vr.IsValid)
            return vr;

        return await database.TransactionAsync(Language.Save, async db =>
        {
            model.Role = string.Empty;
            await db.DeleteAsync<SysUserRole>(d => d.UserId == model.Id);
            if (roles != null && roles.Count > 0)
            {
                model.Role = string.Join(",", roles.Select(r => r.Name).ToArray());
                foreach (var item in roles)
                {
                    await db.InsertAsync(new SysUserRole { UserId = model.Id, RoleId = item.Id });
                }
            }
            await db.SaveAsync(model);
        }, model);
    }
}