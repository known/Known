namespace Known;

public static class AdmExtension
{
    public static void AddKnownAdmin(this IServiceCollection services)
    {
        var assembly = typeof(AdmExtension).Assembly;
        Config.AddModule(assembly);
        // 配置UI
        //UIConfig.TopNavType = typeof(KTopNavbar);
        //UIConfig.ModulePageType = typeof(ModuleList);
        //UIConfig.EnableEdit = false;
        //企业信息
        AdminConfig.CompanyTabs.Set<CompanyBaseInfo>(1, Language.BasicInfo);
        //关于系统
        UIConfig.SystemTabs.Set<SysSystemInfo>(1, Language.SystemInfo);
        UIConfig.SystemTabs.Set<SecuritySetting>(2, Language.SecuritySetting);
        // 添加样式
        KStyleSheet.AddStyle("_content/Known.Admin/css/web.css");
    }

    public static void AddKnownAdminClient(this IServiceCollection services)
    {
    }

    public static void AddKnownAdminCore(this IServiceCollection services)
    {
        //AppData.Enabled = false;
        CoreConfig.OnInstall = AdminHelper.Install;
        //CoreConfig.OnInstallModules = OnInstallModules;
        //CoreConfig.OnInitialModules = OnInitialModules;
        CoreConfig.OnCodeTable = db => db.GetDictionariesAsync();
        CoreConfig.OnRoleModule = (db, id) => db.GetRoleModuleIdsAsync(id);
        AdminExtension.Service = new AdminService();
        UserExtension.OnSyncUser = (db, info) => db.SyncSysUserAsync(info);
        UserExtension.OnUserSystem = (db, user) => db.GetUserSystemAsync(user);
        UserExtension.OnUserOrgName = (db, user) => db.GetUserOrgNameAsync(user);

        //内置模块
        // 添加默认一级模块
        Config.Modules.AddItem("0", Constants.BaseData, AdminLanguage.BaseData, "database", 1);
        Config.Modules.AddItem("0", Constants.System, AdminLanguage.SystemManage, "setting", 99);

        // 添加模型
        DbConfig.Models.Add<SysRoleModule>(x => new { x.RoleId, x.ModuleId });
        DbConfig.Models.Add<SysUserRole>(x => new { x.UserId, x.RoleId });
    }

    //private static async Task OnInstallModules(Database db)
    //{
    //    AppData.LoadAppData();
    //    var modules = AppData.Data.Modules?.Select(m => SysModule1.Load(db.User, m)).OrderBy(m => m.ParentId).ThenBy(m => m.Sort).ToList();
    //    await db.DeleteAllAsync<SysModule1>();
    //    foreach (var item in modules)
    //    {
    //        var parent = modules.FirstOrDefault(m => m.Code == item.ParentId);
    //        item.ParentId = parent?.Id ?? "0";
    //        await db.InsertAsync(item, false);
    //    }
    //}

    //private static async Task<List<ModuleInfo>> OnInitialModules(Database db)
    //{
    //    var modules = new List<ModuleInfo>();
    //    var items = await db.QueryListAsync<SysModule1>();
    //    if (items != null && items.Count > 0)
    //    {
    //        foreach (var item in items.OrderBy(m => m.Sort))
    //        {
    //            modules.Add(item.ToModuleInfo());
    //        }
    //        DataHelper.Initialize(modules);
    //    }
    //    return modules;
    //}
}