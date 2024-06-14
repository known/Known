namespace Known.Services;

public interface ISystemService : IService
{
    Task<PagingResult<SysTask>> QueryTasksAsync(PagingCriteria criteria);
    Task<PagingResult<SysLog>> QueryLogsAsync(PagingCriteria criteria);
    Task<InstallInfo> GetInstallAsync();
    Task<SystemInfo> GetSystemAsync();
    Task<SysModule> GetModuleAsync(string id);
    Task<Result> SaveInstallAsync(InstallInfo info);
    Task<Result> SaveSystemAsync(SystemInfo info);
    Task<Result> SaveKeyAsync(SystemInfo info);
    Task AddLogAsync(SysLog log);
}

class SystemService(Context context) : ServiceBase(context), ISystemService
{
    internal const string KeySystem = "SystemInfo";

    //Config
    public static async Task<T> GetConfigAsync<T>(Database db, string key)
    {
        var json = await SystemRepository.GetConfigAsync(db, key);
        return Utils.FromJson<T>(json);
    }

    public static async Task SaveConfigAsync(Database db, string key, object value)
    {
        var json = Utils.ToJson(value);
        await SystemRepository.SaveConfigAsync(db, key, json);
    }

    //public async Task<T> GetConfigAsync<T>(string key)
    //{
    //    var json = await SystemRepository.GetConfigAsync(Database, key);
    //    return Utils.FromJson<T>(json);
    //}

    //public async Task<Result> SaveConfigAsync(ConfigInfo info)
    //{
    //    await Platform.System.SaveConfigAsync(Database, info.Key, info.Value);
    //    return Result.Success(Language.Success(Language.Save));
    //}

    //Install
    public async Task<InstallInfo> GetInstallAsync()
    {
        var info = GetInstall();
        Config.System = await GetSystemAsync(Database);
        info.IsInstalled = Config.System != null;
        //await Platform.Dictionary.RefreshCacheAsync();
        await CheckKeyAsync();
        return info;
    }

    //public async Task<Result> UpdateKeyAsync(InstallInfo info)
    //{
    //    if (info == null)
    //        return Result.Error(Language["Tip.InstallRequired"]);

    //    if (!Utils.HasNetwork())
    //        return Result.Error(Language["Tip.NotNetwork"]);

    //    var result = await PlatformHelper.UpdateKeyAsync?.Invoke(info);
    //    return result ?? Result.Success("");
    //    return Result.Success("");
    //}

    public async Task<Result> SaveInstallAsync(InstallInfo info)
    {
        if (info == null)
            return Result.Error(Language["Tip.InstallRequired"]);

        if (info.AdminPassword != info.Password1)
            return Result.Error(Language["Tip.PwdNotEqual"]);

        var modules = GetModules();
        var company = GetCompany(info);
        var user = GetUser(info);
        var orga = GetOrganization(info);
        var sys = GetSystem(info);
        var path = GetProductKeyPath();
        Utils.SaveFile(path, info.ProductKey);

        var database = new Database
        {
            Context = Context,
            User = new UserInfo { AppId = user.AppId, CompNo = user.CompNo, UserName = user.UserName, Name = user.Name }
        };
        var result = await database.TransactionAsync(Language["Install"], async db =>
        {
            await SaveConfigAsync(db, KeySystem, sys);
            await db.SaveDatasAsync(modules);
            await db.SaveAsync(company);
            await db.SaveAsync(user);
            await db.SaveAsync(orga);
        });
        if (result.IsValid)
            result.Data = await GetInstallAsync();
        return result;
    }

    //System
    public async Task<SystemInfo> GetSystemAsync()
    {
        var info = await GetSystemAsync(Database);
        if (info != null)
        {
            var install = GetInstall();
            info.ProductId = install.ProductId;
            info.ProductKey = install.ProductKey;
        }
        return info;
    }

    internal static async Task<SystemInfo> GetSystemAsync(Database db)
    {
        if (!Config.App.IsPlatform || db.User == null)
        {
            var json = await SystemRepository.GetConfigAsync(db, KeySystem);
            return Utils.FromJson<SystemInfo>(json);
        }

        var company = await CompanyRepository.GetCompanyAsync(db);
        if (company == null)
            return GetSystem(db.User);

        return Utils.FromJson<SystemInfo>(company.SystemData);
    }

    public async Task<Result> SaveKeyAsync(SystemInfo info)
    {
        var path = GetProductKeyPath();
        Utils.SaveFile(path, info.ProductKey);
        await SaveConfigAsync(Database, KeySystem, info);
        return await CheckKeyAsync();
    }

    public async Task<Result> SaveSystemAsync(SystemInfo info)
    {
        if (Config.App.IsPlatform)
        {
            var company = await CompanyRepository.GetCompanyAsync(Database);
            if (company == null)
                return Result.Error(Language["Tip.CompanyNotExists"]);

            company.SystemData = Utils.ToJson(info);
            await Database.SaveAsync(company);
        }
        else
        {
            await SaveConfigAsync(Database, KeySystem, info);
        }

        Config.System = info;
        return Result.Success(Language.Success(Language.Save));
    }

    private static InstallInfo GetInstall()
    {
        var app = Config.App;
        var path = GetProductKeyPath();
        var info = new InstallInfo
        {
            AppName = app.Name,
            ProductId = app.ProductId,
            ProductKey = Utils.ReadFile(path),
            AdminName = Constants.SysUserName
        };
        return info;
    }

    private static SystemInfo GetSystem(InstallInfo info)
    {
        return new SystemInfo
        {
            CompNo = info.CompNo,
            CompName = info.CompName,
            AppName = info.AppName,
            ProductId = info.ProductId,
            ProductKey = info.ProductKey,
            UserDefaultPwd = "888888"
        };
    }

    private static SystemInfo GetSystem(UserInfo info)
    {
        return new SystemInfo
        {
            CompNo = info.CompNo,
            CompName = info.CompName,
            AppName = Config.App.Name,
            UserDefaultPwd = "888888"
        };
    }

    //Module
    public Task<SysModule> GetModuleAsync(string id) => Database.QueryByIdAsync<SysModule>(id);

    private static List<SysModule> GetModules()
    {
        var modules = new List<SysModule>();
        var baseData = GetModule("BaseData", "基础数据", "database", ModuleType.Menu.ToString(), 1);
        modules.Add(baseData);
        modules.Add(GetSysDictionary(baseData.Id));
        modules.Add(GetSysOrganization(baseData.Id));

        var system = GetModule("System", "系统管理", "setting", ModuleType.Menu.ToString(), 2);
        modules.Add(system);
        modules.Add(GetSysSystem(system.Id));
        modules.Add(GetSysRole(system.Id));
        modules.Add(GetSysUser(system.Id));
        modules.Add(GetSysTask(system.Id));
        modules.Add(GetSysFile(system.Id));
        modules.Add(GetSysLog(system.Id));
        modules.Add(GetSysModule(system.Id));

        return modules;
    }

    private static SysModule GetModule(string code, string name, string icon, string target, int sort)
    {
        return new SysModule { ParentId = "0", Code = code, Name = name, Icon = icon, Target = target, Sort = sort, Enabled = true };
    }

    private static SysModule GetSysDictionary(string parentId)
    {
        return new SysModule
        {
            ParentId = parentId,
            Code = "SysDictionaryList",
            Name = "数据字典",
            Icon = "unordered-list",
            Description = "维护系统所需的下拉框数据源。",
            Target = ModuleType.Page.ToString(),
            Url = "/sys/dictionaries",
            Sort = 1,
            Enabled = true,
            EntityData = @"数据字典|SysDictionary
类别|Category|Text|50|Y
类别名称|CategoryName|Text|50
代码|Code|Text|100|Y
名称|Name|Text|150
顺序|Sort|Number
状态|Enabled|Switch
备注|Note|Text|500
子字典|Child|TextArea",
            PageData = "{\"Type\":null,\"ShowPager\":true,\"FixedWidth\":null,\"FixedHeight\":null,\"Tools\":[\"New\",\"DeleteM\",\"Import\",\"AddCategory\"],\"Actions\":[\"Edit\",\"Delete\"],\"Columns\":[{\"Id\":\"Category\",\"Name\":\"类别\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":150,\"Align\":null},{\"Id\":\"Code\",\"Name\":\"代码\",\"IsViewLink\":true,\"IsQuery\":true,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":true,\"DefaultSort\":null,\"Fixed\":null,\"Width\":120,\"Align\":null},{\"Id\":\"Name\",\"Name\":\"名称\",\"IsViewLink\":false,\"IsQuery\":true,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":true,\"DefaultSort\":null,\"Fixed\":null,\"Width\":120,\"Align\":null},{\"Id\":\"Sort\",\"Name\":\"顺序\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":true,\"DefaultSort\":\"升序\",\"Fixed\":null,\"Width\":60,\"Align\":\"center\"},{\"Id\":\"Enabled\",\"Name\":\"状态\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":60,\"Align\":\"center\"},{\"Id\":\"Note\",\"Name\":\"备注\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":200,\"Align\":null}]}",
            FormData = "{\"LabelSpan\":null,\"WrapperSpan\":null,\"Fields\":[{\"Row\":1,\"Column\":1,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Code\",\"Name\":\"代码\",\"Type\":0,\"Length\":null,\"Required\":true},{\"Row\":1,\"Column\":1,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Name\",\"Name\":\"名称\",\"Type\":0,\"Length\":null,\"Required\":false},{\"Row\":1,\"Column\":1,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Sort\",\"Name\":\"顺序\",\"Type\":3,\"Length\":null,\"Required\":true},{\"Row\":1,\"Column\":1,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Enabled\",\"Name\":\"状态\",\"Type\":4,\"Length\":null,\"Required\":true},{\"Row\":1,\"Column\":1,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Note\",\"Name\":\"备注\",\"Type\":1,\"Length\":null,\"Required\":false}]}"
        };
    }

    private static SysModule GetSysOrganization(string parentId)
    {
        return new SysModule
        {
            ParentId = parentId,
            Code = "SysOrganizationList",
            Name = "组织架构",
            Icon = "partition",
            Description = "维护企业组织架构信息。",
            Target = ModuleType.Page.ToString(),
            Url = "/sys/organizations",
            Sort = 2,
            Enabled = true,
            EntityData = @"组织架构|SysOrganization
上级组织|ParentId|Text|50
编码|Code|Text|50|Y
名称|Name|Text|50|Y
管理者|ManagerId|Text|50
备注|Note|Text|500",
            PageData = "{\"Type\":null,\"ShowPager\":false,\"FixedWidth\":null,\"FixedHeight\":null,\"Tools\":[\"New\",\"DeleteM\"],\"Actions\":[\"Edit\",\"Delete\"],\"Columns\":[{\"Id\":\"Code\",\"Name\":\"编码\",\"IsViewLink\":true,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":100,\"Align\":null},{\"Id\":\"Name\",\"Name\":\"名称\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":150,\"Align\":null},{\"Id\":\"Note\",\"Name\":\"备注\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":400,\"Align\":null}]}",
            FormData = "{\"LabelSpan\":null,\"WrapperSpan\":null,\"Fields\":[{\"Row\":1,\"Column\":1,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Code\",\"Name\":\"编码\",\"Type\":0,\"Length\":null,\"Required\":true},{\"Row\":2,\"Column\":1,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Name\",\"Name\":\"名称\",\"Type\":0,\"Length\":null,\"Required\":true},{\"Row\":3,\"Column\":1,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Note\",\"Name\":\"备注\",\"Type\":1,\"Length\":null,\"Required\":false}]}"
        };
    }

    private static SysModule GetSysSystem(string parentId)
    {
        return new SysModule
        {
            ParentId = parentId,
            Code = "SysSystem",
            Name = "关于系统",
            Icon = "info-circle",
            Description = "显示系统版本及产品授权信息。",
            Target = ModuleType.Custom.ToString(),
            Url = "/sys/info",
            Sort = 1,
            Enabled = true
        };
    }

    private static SysModule GetSysRole(string parentId)
    {
        return new SysModule
        {
            ParentId = parentId,
            Code = "SysRoleList",
            Name = "角色管理",
            Icon = "team",
            Description = "维护系统用户角色及其菜单权限信息。",
            Target = ModuleType.Page.ToString(),
            Url = "/sys/roles",
            Sort = 2,
            Enabled = true,
            EntityData = @"系统角色|SysRole
名称|Name|Text|50|Y
状态|Enabled|Switch
备注|Note|TextArea",
            PageData = "{\"Type\":null,\"ShowPager\":true,\"FixedWidth\":null,\"FixedHeight\":null,\"Tools\":[\"New\",\"DeleteM\"],\"Actions\":[\"Edit\",\"Delete\"],\"Columns\":[{\"Id\":\"Name\",\"Name\":\"名称\",\"IsViewLink\":true,\"IsQuery\":true,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":120,\"Align\":null},{\"Id\":\"Enabled\",\"Name\":\"状态\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":60,\"Align\":\"center\"},{\"Id\":\"Note\",\"Name\":\"备注\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":500,\"Align\":null}]}",
            FormData = "{\"Width\":1000,\"Maximizable\":false,\"DefaultMaximized\":false,\"LabelSpan\":null,\"WrapperSpan\":null,\"Fields\":[{\"Row\":1,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Name\",\"Name\":\"名称\",\"Type\":0,\"Length\":null,\"Required\":true},{\"Row\":1,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Enabled\",\"Name\":\"状态\",\"Type\":4,\"Length\":null,\"Required\":true},{\"Row\":1,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Note\",\"Name\":\"备注\",\"Type\":1,\"Length\":null,\"Required\":false}]}"
        };
    }

    private static SysModule GetSysUser(string parentId)
    {
        return new SysModule
        {
            ParentId = parentId,
            Code = "SysUserList",
            Name = "用户管理",
            Icon = "user",
            Description = "维护系统用户账号及角色信息。",
            Target = ModuleType.Page.ToString(),
            Url = "/sys/users",
            Sort = 3,
            Enabled = true,
            EntityData = @"系统用户|SysUser
组织编码|OrgNo|Text|50
用户名|UserName|Text|50|Y
密码|Password|Text|50|Y
姓名|Name|Text|50
英文名|EnglishName|Text|50
性别|Gender|Text|50
固定电话|Phone|Text|50
移动电话|Mobile|Text|50
电子邮件|Email|Text|50
状态|Enabled|Switch
简介|Note|TextArea|500
首次登录时间|FirstLoginTime|Date
首次登录IP|FirstLoginIP|Text|50
最近登录时间|LastLoginTime|Date
最近登录IP|LastLoginIP|Text|50
类型|Type|Text|500
角色|Role|Text|500
数据|Data|Text",
            PageData = "{\"Type\":null,\"ShowPager\":true,\"FixedWidth\":null,\"FixedHeight\":null,\"Tools\":[\"New\",\"DeleteM\",\"Enable\",\"Disable\",\"ResetPassword\",\"ChangeDepartment\"],\"Actions\":[\"Edit\",\"Delete\"],\"Columns\":[{\"Id\":\"UserName\",\"Name\":\"用户名\",\"IsViewLink\":true,\"IsQuery\":true,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":120,\"Align\":null},{\"Id\":\"Name\",\"Name\":\"姓名\",\"IsViewLink\":false,\"IsQuery\":true,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":100,\"Align\":null},{\"Id\":\"Gender\",\"Name\":\"性别\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":60,\"Align\":null},{\"Id\":\"Mobile\",\"Name\":\"移动电话\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":120,\"Align\":null},{\"Id\":\"Email\",\"Name\":\"电子邮件\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":120,\"Align\":null},{\"Id\":\"Enabled\",\"Name\":\"状态\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":60,\"Align\":\"center\"},{\"Id\":\"Role\",\"Name\":\"角色\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":200,\"Align\":null}]}",
            FormData = "{\"Width\":800,\"Maximizable\":false,\"DefaultMaximized\":false,\"LabelSpan\":null,\"WrapperSpan\":null,\"Fields\":[{\"Row\":1,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"UserName\",\"Name\":\"用户名\",\"Type\":0,\"Length\":null,\"Required\":true},{\"Row\":1,\"Column\":2,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Name\",\"Name\":\"姓名\",\"Type\":0,\"Length\":null,\"Required\":true},{\"Row\":2,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"EnglishName\",\"Name\":\"英文名\",\"Type\":0,\"Length\":null,\"Required\":false},{\"Row\":2,\"Column\":2,\"CategoryType\":null,\"Category\":\"GenderType\",\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Gender\",\"Name\":\"性别\",\"Type\":7,\"Length\":null,\"Required\":false},{\"Row\":3,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Phone\",\"Name\":\"固定电话\",\"Type\":0,\"Length\":null,\"Required\":false},{\"Row\":3,\"Column\":2,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Mobile\",\"Name\":\"移动电话\",\"Type\":0,\"Length\":null,\"Required\":false},{\"Row\":4,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Email\",\"Name\":\"电子邮件\",\"Type\":0,\"Length\":null,\"Required\":false},{\"Row\":4,\"Column\":2,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Enabled\",\"Name\":\"状态\",\"Type\":4,\"Length\":null,\"Required\":true}]}"
        };
    }

    private static SysModule GetSysTask(string parentId)
    {
        return new SysModule
        {
            ParentId = parentId,
            Code = "SysTaskList",
            Name = "后台任务",
            Icon = "control",
            Description = "查询系统所有定时任务运行情况。",
            Target = ModuleType.Page.ToString(),
            Url = "/sys/tasks",
            Sort = 4,
            Enabled = true,
            EntityData = @"系统任务|SysTask
业务ID|BizId|Text|50|Y
类型|Type|Text|50
名称|Name|Text|50
执行目标|Target|Text|200
执行状态|Status|Text|50|Y
开始时间|BeginTime|Date
结束时间|EndTime|Date
备注|Note|Text",
            PageData = "{\"Type\":null,\"ShowPager\":true,\"FixedWidth\":null,\"FixedHeight\":null,\"Tools\":null,\"Actions\":null,\"Columns\":[{\"Id\":\"Type\",\"Name\":\"类型\",\"IsViewLink\":false,\"IsQuery\":true,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":true,\"DefaultSort\":null,\"Fixed\":null,\"Width\":100,\"Align\":null},{\"Id\":\"Name\",\"Name\":\"名称\",\"IsViewLink\":false,\"IsQuery\":true,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":150,\"Align\":null},{\"Id\":\"Target\",\"Name\":\"执行目标\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":180,\"Align\":null},{\"Id\":\"Status\",\"Name\":\"执行状态\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":100,\"Align\":null},{\"Id\":\"BeginTime\",\"Name\":\"开始时间\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":true,\"DefaultSort\":\"Descend\",\"Fixed\":null,\"Width\":150,\"Align\":\"center\"},{\"Id\":\"EndTime\",\"Name\":\"结束时间\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":150,\"Align\":\"center\"},{\"Id\":\"Note\",\"Name\":\"备注\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":200,\"Align\":null}]}"
        };
    }

    private static SysModule GetSysFile(string parentId)
    {
        return new SysModule
        {
            ParentId = parentId,
            Code = "SysFileList",
            Name = "系统附件",
            Icon = "file",
            Description = "查询系统所有附件信息。",
            Target = ModuleType.Page.ToString(),
            Url = "/sys/files",
            Sort = 5,
            Enabled = true,
            EntityData = @"系统文件|SysFile
一级分类|Category1|Text|50|Y
二级分类|Category2|Text|50
文件名称|Name|Text|250|Y
文件类型|Type|Text|50
文件路径|Path|Text|500
文件大小|Size|Number
原文件名|SourceName|Text|250|Y
扩展名|ExtName|Text|50|Y
备注|Note|Text|500
业务ID|BizId|Text|50
文件缩略图路径|ThumbPath|Text|500",
            PageData = "{\"Type\":null,\"ShowPager\":true,\"FixedWidth\":null,\"FixedHeight\":null,\"Tools\":null,\"Actions\":null,\"Columns\":[{\"Id\":\"Category1\",\"Name\":\"一级分类\",\"IsViewLink\":false,\"IsQuery\":true,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":100,\"Align\":null},{\"Id\":\"Category2\",\"Name\":\"二级分类\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":100,\"Align\":null},{\"Id\":\"Name\",\"Name\":\"文件名称\",\"IsViewLink\":false,\"IsQuery\":true,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":180,\"Align\":null},{\"Id\":\"Type\",\"Name\":\"文件类型\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":150,\"Align\":null},{\"Id\":\"Size\",\"Name\":\"文件大小\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":80,\"Align\":null},{\"Id\":\"SourceName\",\"Name\":\"原文件名\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":180,\"Align\":null},{\"Id\":\"ExtName\",\"Name\":\"扩展名\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":80,\"Align\":null},{\"Id\":\"Note\",\"Name\":\"备注\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":200,\"Align\":null}]}"
        };
    }

    private static SysModule GetSysLog(string parentId)
    {
        return new SysModule
        {
            ParentId = parentId,
            Code = "SysLogList",
            Name = "系统日志",
            Icon = "clock-circle",
            Description = "查询系统用户操作日志信息。",
            Target = ModuleType.Page.ToString(),
            Url = "/sys/logs",
            Sort = 6,
            Enabled = true,
            EntityData = @"系统日志|SysLog
操作类型|Type|Text|50|Y
操作对象|Target|Text|50|Y
操作内容|Content|Text",
            PageData = "{\"Type\":null,\"ShowPager\":true,\"FixedWidth\":null,\"FixedHeight\":null,\"Tools\":null,\"Actions\":null,\"Columns\":[{\"Id\":\"Type\",\"Name\":\"操作类型\",\"IsViewLink\":false,\"IsQuery\":true,\"IsQueryAll\":true,\"IsSum\":false,\"IsSort\":true,\"DefaultSort\":null,\"Fixed\":null,\"Width\":\"100\",\"Align\":null},{\"Id\":\"Target\",\"Name\":\"操作对象\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":true,\"DefaultSort\":null,\"Fixed\":null,\"Width\":\"150\",\"Align\":null},{\"Id\":\"Content\",\"Name\":\"操作内容\",\"IsViewLink\":false,\"IsQuery\":true,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":null,\"Align\":null},{\"Id\":\"CreateBy\",\"Name\":\"创建人\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":true,\"DefaultSort\":null,\"Fixed\":null,\"Width\":\"100\",\"Align\":null},{\"Id\":\"CreateTime\",\"Name\":\"创建时间\",\"IsViewLink\":false,\"IsQuery\":true,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":true,\"DefaultSort\":\"Descend\",\"Fixed\":null,\"Width\":\"150\",\"Align\":null}]}"
        };
    }

    private static SysModule GetSysModule(string parentId)
    {
        return new SysModule
        {
            ParentId = parentId,
            Code = "SysModuleList",
            Name = "模块管理",
            Icon = "appstore-add",
            Description = "维护系统菜单按钮及列表栏位信息。",
            Target = ModuleType.Page.ToString(),
            Url = "/sys/modules",
            Sort = 7,
            Enabled = true,
            EntityData = @"系统模块|SysModule
上级|ParentId|Text|50
代码|Code|Text|50|Y
名称|Name|Text|50|Y
图标|Icon|Text|50
描述|Description|Text|200
类型|Target|Text|50
URL|Url|Text|200
顺序|Sort|Number
可用|Enabled|Switch
实体设置|EntityData|Text
页面设置|PageData|Text
表单设置|FormData|Text
备注|Note|Text|500",
            PageData = "{\"Type\":null,\"ShowPager\":false,\"FixedWidth\":null,\"FixedHeight\":null,\"Tools\":[\"New\",\"DeleteM\",\"Copy\",\"Move\"],\"Actions\":[\"Edit\",\"Delete\",\"MoveUp\",\"MoveDown\"],\"Columns\":[{\"Id\":\"Code\",\"Name\":\"代码\",\"IsViewLink\":true,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":130,\"Align\":null},{\"Id\":\"Name\",\"Name\":\"名称\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":120,\"Align\":null},{\"Id\":\"Description\",\"Name\":\"描述\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":180,\"Align\":null},{\"Id\":\"Target\",\"Name\":\"类型\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":80,\"Align\":\"center\"},{\"Id\":\"Url\",\"Name\":\"Url\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":150,\"Align\":null},{\"Id\":\"Sort\",\"Name\":\"顺序\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":60,\"Align\":\"center\"},{\"Id\":\"Enabled\",\"Name\":\"可用\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":60,\"Align\":\"center\"},{\"Id\":\"Note\",\"Name\":\"备注\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":150,\"Align\":null}]}",
            FormData = "{\"Width\":1200,\"Maximizable\":true,\"DefaultMaximized\":false,\"LabelSpan\":null,\"WrapperSpan\":null,\"Fields\":[{\"Row\":1,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Code\",\"Name\":\"代码\",\"Type\":0,\"Length\":null,\"Required\":true},{\"Row\":1,\"Column\":2,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Name\",\"Name\":\"名称\",\"Type\":0,\"Length\":null,\"Required\":true},{\"Row\":2,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Icon\",\"Name\":\"图标\",\"Type\":0,\"Length\":null,\"Required\":false},{\"Row\":4,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Description\",\"Name\":\"描述\",\"Type\":0,\"Length\":null,\"Required\":false},{\"Row\":2,\"Column\":2,\"CategoryType\":\"Custom\",\"Category\":\"ModuleType\",\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Target\",\"Name\":\"类型\",\"Type\":7,\"Length\":null,\"Required\":true},{\"Row\":3,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Url\",\"Name\":\"URL\",\"Type\":0,\"Length\":null,\"Required\":false},{\"Row\":3,\"Column\":2,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Enabled\",\"Name\":\"可用\",\"Type\":4,\"Length\":null,\"Required\":true},{\"Row\":5,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"Note\",\"Name\":\"备注\",\"Type\":1,\"Length\":null,\"Required\":false}]}"
        };
    }

    private static SysCompany GetCompany(InstallInfo info)
    {
        var sys = GetSystem(info);
        return new SysCompany
        {
            AppId = Config.App.Id,
            CompNo = info.CompNo,
            Code = info.CompNo,
            Name = info.CompName,
            SystemData = Utils.ToJson(sys)
        };
    }

    private static SysUser GetUser(InstallInfo info)
    {
        return new SysUser
        {
            AppId = Config.App.Id,
            CompNo = info.CompNo,
            OrgNo = info.CompNo,
            UserName = info.AdminName.ToLower(),
            Password = Utils.ToMd5(info.AdminPassword),
            Name = info.AdminName,
            EnglishName = info.AdminName,
            Gender = "Male",
            Role = "Admin",
            Enabled = true
        };
    }

    private static SysOrganization GetOrganization(InstallInfo info)
    {
        return new SysOrganization
        {
            AppId = Config.App.Id,
            CompNo = info.CompNo,
            ParentId = "0",
            Code = info.CompNo,
            Name = info.CompName
        };
    }

    private static string GetProductKeyPath()
    {
        var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(path, "Known", $"{Config.App.Id}.key");
    }

    private async Task<Result> CheckKeyAsync()
    {
        var info = await GetSystemAsync();
        return Config.App.CheckSystemInfo(info);
    }

    //Task
    public Task<PagingResult<SysTask>> QueryTasksAsync(PagingCriteria criteria)
    {
        if (criteria.OrderBys == null || criteria.OrderBys.Length == 0)
            criteria.OrderBys = [$"{nameof(SysTask.CreateTime)} desc"];
        return SystemRepository.QueryTasksAsync(Database, criteria);
    }

    //Log
    public Task<PagingResult<SysLog>> QueryLogsAsync(PagingCriteria criteria)
    {
        if (criteria.OrderBys == null || criteria.OrderBys.Length == 0)
            criteria.OrderBys = [$"{nameof(SysLog.CreateTime)} desc"];
        return SystemRepository.QueryLogsAsync(Database, criteria);
    }

    //internal async Task<Result> DeleteLogsAsync(string data)
    //{
    //    var ids = Utils.FromJson<string[]>(data);
    //    var entities = await Database.QueryListByIdAsync<SysLog>(ids);
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

    public Task AddLogAsync(SysLog log) => Database.SaveAsync(log);
}