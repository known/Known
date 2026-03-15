namespace Known.Services;

/// <summary>
/// 企业信息服务接口。
/// </summary>
public interface ICompanyService : IService
{
    /// <summary>
    /// 异步获取企业信息。
    /// </summary>
    /// <returns></returns>
    Task<string> GetCompanyAsync();

    /// <summary>
    /// 异步保存企业信息。
    /// </summary>
    /// <param name="model">企业信息。</param>
    /// <returns></returns>
    Task<Result> SaveCompanyAsync(object model);

    /// <summary>
    /// 异步分页查询租户。
    /// </summary>
    /// <param name="criteria">查询条件。</param>
    /// <returns></returns>
    Task<PagingResult<SysCompany>> QueryTenantsAsync(PagingCriteria criteria);

    /// <summary>
    /// 异步获取租户信息。
    /// </summary>
    /// <param name="id">租户ID。</param>
    /// <returns></returns>
    Task<SysCompany> GetTenantAsync(string id);

    /// <summary>
    /// 异步删除租户。
    /// </summary>
    /// <param name="infos">租户列表。</param>
    /// <returns></returns>
    Task<Result> DeleteTenantsAsync(List<SysCompany> infos);

    /// <summary>
    /// 异步保存租户。
    /// </summary>
    /// <param name="info">租户信息。</param>
    /// <returns></returns>
    Task<Result> SaveTenantAsync(UploadInfo<SysCompany> info);
}

[Client]
class CompanyClient(HttpClient http) : ClientBase(http), ICompanyService
{
    public Task<string> GetCompanyAsync() => Http.GetTextAsync("/Company/GetCompany");
    public Task<Result> SaveCompanyAsync(object model) => Http.PostAsync("/Company/SaveCompany", model);
    public Task<PagingResult<SysCompany>> QueryTenantsAsync(PagingCriteria criteria) => Http.QueryAsync<SysCompany>("/Company/QueryTenants", criteria);
    public Task<SysCompany> GetTenantAsync(string id) => Http.GetAsync<SysCompany>($"/Company/GetTenant?id={id}");
    public Task<Result> DeleteTenantsAsync(List<SysCompany> infos) => Http.PostAsync("/Company/DeleteTenants", infos);
    public Task<Result> SaveTenantAsync(UploadInfo<SysCompany> info) => Http.PostAsync("/Company/SaveTenant", info);
}

[WebApi, Service]
class CompanyService(Context context) : ServiceBase(context), ICompanyService
{
    private const string KeyCompany = "CompanyInfo";

    public async Task<string> GetCompanyAsync()
    {
        var database = Database;
        if (Config.App.IsPlatform)
        {
            return await GetCompanyDataAsync(database);
        }
        else
        {
            var json = await database.GetConfigAsync(KeyCompany);
            if (string.IsNullOrEmpty(json))
                json = GetDefaultData(database.User);
            return json;
        }
    }

    public async Task<Result> SaveCompanyAsync(object model)
    {
        var database = Database;
        if (Config.App.IsPlatform)
        {
            var result = await database.SaveCompanyDataAsync(CurrentUser.CompNo, model);
            if (!result.IsValid)
                return result;
        }
        else
        {
            await database.SaveConfigAsync(KeyCompany, model);
        }
        return Result.Success(Language.SaveSuccess);
    }

    public Task<PagingResult<SysCompany>> QueryTenantsAsync(PagingCriteria criteria)
    {
        if (!IsSystemAdmin())
            return Task.FromResult(new PagingResult<SysCompany> { Message = "无权限访问！" });

        return Database.QueryPageAsync<SysCompany>(criteria);
    }

    public async Task<SysCompany> GetTenantAsync(string id)
    {
        if (!IsSystemAdmin())
            return null;

        var info = await Database.QueryByIdAsync<SysCompany>(id);
        info ??= new SysCompany();
        info.ConnTypes = string.Join(",", DatabaseOption.Types);
        return info;
    }

    public async Task<Result> DeleteTenantsAsync(List<SysCompany> infos)
    {
        if (!IsSystemAdmin())
            return Result.Error("无权限访问！");

        if (infos == null || infos.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        var database = Database;
        foreach (var item in infos)
        {
            if (string.Equals(item.Code, CurrentUser?.CompNo, StringComparison.OrdinalIgnoreCase))
                return Result.Error("不能删除当前租户！");

            var userName = item.Code.ToLower();
            if (await database.ExistsAsync<SysUser>(d => d.CompNo == item.Code && d.UserName != userName))
                return Result.Error($"租户[{item.Code}]下存在用户，无法删除！");
        }

        var oldFiles = new List<string>();
        var result = await database.TransactionAsync(Language.Delete, async db =>
        {
            foreach (var item in infos)
            {
                if (!string.IsNullOrWhiteSpace(item.SystemData.LogoPath))
                    oldFiles.Add(item.SystemData.LogoPath);
                await db.DeleteAsync<SysCompany>(item.Id);
                await db.DeleteAsync<SysOrganization>(d => d.CompNo == item.Code);
                await db.DeleteAsync<SysUser>(d => d.CompNo == item.Code);
                await db.DeleteAsync<SysLog>(d => d.CompNo == item.Code);
            }
        });
        if (result.IsValid)
            AttachFile.DeleteFiles(oldFiles);
        return result;
    }

    public async Task<Result> SaveTenantAsync(UploadInfo<SysCompany> info)
    {
        if (!IsSystemAdmin())
            return Result.Error("无权限访问！");

        if (info == null)
            return Result.Error("保存数据不能为空！");

        var database = Database;
        var model = await database.QueryByIdAsync<SysCompany>(info.Model.Id);
        model ??= new SysCompany();

        var code = model.Code;
        model.FillModel(info.Model);

        if (!model.IsNew && !string.Equals(code, model.Code, StringComparison.OrdinalIgnoreCase))
            return Result.Error("企业编码不允许修改！");
        if (string.IsNullOrWhiteSpace(model.Code))
            return Result.Error("企业编码不能为空！");

        model.CompNo = model.Code;

        var userName = model.Code.ToLower();
        var vr = model.Validate(Context);
        if (vr.IsValid)
        {
            if (await database.ExistsAsync<SysCompany>(d => d.Id != model.Id && d.Code == model.Code))
                vr.AddError("企业编码已存在！");
            if (await database.ExistsAsync<SysUser>(d => d.CompNo != model.Code && d.UserName == userName))
                vr.AddError("企业编码用户账号已存在！");
        }
        if (!vr.IsValid)
            return vr;

        var fileFiles = info.Files?.GetAttachFiles(nameof(SystemInfo.LogoPath), "Logos");
        return await database.TransactionAsync(Language.Save, async db =>
        {
            if (model.IsNew)
            {
                model.SystemData = new SystemInfo
                {
                    CompNo = model.Code,
                    CompName = model.Name,
                    AppName = Config.App.Name
                };
                await db.SaveOrganizationAsync(model);
                await db.SaveUserAsync(model);
            }

            if (fileFiles != null && fileFiles.Count > 0)
            {
                var attach = fileFiles[0];
                attach.FilePath = $"Logos/{model.Code}{attach.ExtName}";
                var url = Config.GetFileUrl(attach.FilePath);
                model.SystemData.LogoPath = url;
                await attach.SaveAsync();
            }

            await db.SaveAsync(model);
            info.Model.Id = model.Id;
        }, info.Model);
    }

    private static async Task<string> GetCompanyDataAsync(Database db)
    {
        var data = await db.GetCompanyDataAsync(db.User.CompNo);
        if (!string.IsNullOrWhiteSpace(data))
            return data;

        return GetDefaultData(db.User);
    }

    private static string GetDefaultData(UserInfo user)
    {
        return Utils.ToJson(new { Code = user.CompNo, Name = user.CompName });
    }

    private bool IsSystemAdmin() => CurrentUser?.IsSystemAdmin() == true;
}