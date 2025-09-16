namespace Known;

public static class AdminExtension
{
    public static void AddKnownAdmin(this IServiceCollection services)
    {
        var assembly = typeof(AdminExtension).Assembly;
        Config.AddModule(assembly);
        AppData.Enabled = false;

        services.AddServices(assembly);
        Config.OnInstall = AdminHelper.Install;
        //Config.OnInstallModules = OnInstallModules;
        //Config.OnInitialModules = OnInitialModules;
        Config.OnCodeTable = db => db.GetDictionariesAsync();
        Config.OnRoleModule = (db, id) => db.GetRoleModuleIdsAsync(id);
        UserExtension.OnSyncUser = (db, info) => db.SyncSysUserAsync(info);
        UserExtension.OnUserSystem = (db, user) => db.GetUserSystemAsync(user);
        UserExtension.OnUserOrgName = (db, user) => db.GetUserOrgNameAsync(user);
        TaskExtension.OnGetTask = (db, bizId) => db.GetSysTaskAsync(bizId);
        TaskExtension.OnCreateTask = (db, info) => db.CreateSysTaskAsync(info);
        TaskExtension.OnSaveTask = (db, info) => db.SaveSysTaskAsync(info);

        // 配置UI
        //UIConfig.TopNavType = typeof(KTopNavbar);
        //UIConfig.ModulePageType = typeof(ModuleList);
        //UIConfig.EnableEdit = false;
        //企业信息
        AdminConfig.CompanyTabs.Set<CompanyBaseInfo>(1, Language.BasicInfo);
        //关于系统
        UIConfig.SystemTabs.Set<SysSystemInfo>(1, Language.SystemInfo);
        UIConfig.SystemTabs.Set<SysSecuritySetting>(2, Language.SecuritySetting);
        //内置模块
        // 添加默认一级模块
        AppData.Data.Modules.AddItem("0", Constants.BaseData, AdminLanguage.BaseData, "database", 1);
        AppData.Data.Modules.AddItem<CompanyForm>(Constants.BaseData, 1);
        AppData.Data.Modules.AddItem<SysDictionaryList>(Constants.BaseData, 2);
        AppData.Data.Modules.AddItem<SysOrganizationList>(Constants.BaseData, 3);
        AppData.Data.Modules.AddItem("0", Constants.System, AdminLanguage.SystemManage, "setting", 99);
        AppData.Data.Modules.AddItem<SysSystem>(Constants.System, 1);
        AppData.Data.Modules.AddItem<SysRoleList>(Constants.System, 2);
        AppData.Data.Modules.AddItem<SysUserList>(Constants.System, 3);
        AppData.Data.Modules.AddItem<SysTaskList>(Constants.System, 4);
        AppData.Data.Modules.AddItem<SysFileList>(Constants.System, 5);
        AppData.Data.Modules.AddItem<SysLogList>(Constants.System, 6);

        // 添加样式
        KStyleSheet.AddStyle("_content/Known.Admin/css/web.css");

        // 添加模型
        DbConfig.Models.Add<SysRoleModule>(x => new { x.RoleId, x.ModuleId });
        DbConfig.Models.Add<SysUserRole>(x => new { x.UserId, x.RoleId });
    }

    public static void AddKnownAdminClient(this IServiceCollection services)
    {
        var assembly = typeof(AdminExtension).Assembly;
        services.AddClients(assembly);
    }

    private static async Task OnInstallModules(Database db)
    {
        AppData.LoadAppData();
        var modules = AppData.Data.Modules?.Select(m => SysModule1.Load(db.User, m)).OrderBy(m => m.ParentId).ThenBy(m => m.Sort).ToList();
        await db.DeleteAllAsync<SysModule1>();
        foreach (var item in modules)
        {
            var parent = modules.FirstOrDefault(m => m.Code == item.ParentId);
            item.ParentId = parent?.Id ?? "0";
            await db.InsertAsync(item, false);
        }
    }

    private static async Task<List<ModuleInfo>> OnInitialModules(Database db)
    {
        var modules = new List<ModuleInfo>();
        var items = await db.QueryListAsync<SysModule1>();
        if (items != null && items.Count > 0)
        {
            foreach (var item in items.OrderBy(m => m.Sort))
            {
                modules.Add(item.ToModuleInfo());
            }
            DataHelper.Initialize(modules);
        }
        return modules;
    }
}