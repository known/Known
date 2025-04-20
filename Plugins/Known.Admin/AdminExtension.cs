namespace Known;

/// <summary>
/// 依赖注入扩展类。
/// </summary>
public static class AdminExtension
{
    /// <summary>
    /// 添加Known框架后台管理模块。
    /// </summary>
    /// <param name="services">服务集合。</param>
    public static void AddKnownAdmin(this IServiceCollection services)
    {
        Config.IsAdmin = true;

        // 配置UI
        //UIConfig.TopNavType = typeof(KTopNavbar);
        UIConfig.ModulePageType = typeof(ModuleList);
        //UIConfig.EnableEdit = false;

        // 添加样式
        KStyleSheet.AddStyle("_content/Known.Admin/css/web.css");
    }

    /// <summary>
    /// 添加Known框架后台管理模块前端。
    /// </summary>
    /// <param name="services">服务集合。</param>
    public static void AddKnownAdminClient(this IServiceCollection services)
    {
        var assembly = typeof(AdminExtension).Assembly;
        services.AddClients(assembly);
        Config.AddModule(assembly);
    }

    /// <summary>
    /// 添加Known框架后台管理模块后端。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="action">配置选项委托。</param>
    public static void AddKnownAdminCore(this IServiceCollection services, Action<AdminOption> action = null)
    {
        action?.Invoke(AdminOption.Instance);
        AppData.Enabled = false;

        var assembly = typeof(AdminExtension).Assembly;
        services.AddServices(assembly);
        Config.AddModule(assembly);
        Config.OnInstallModules = OnInstallModules;
        Config.OnInitialModules = OnInitialModules;
    }

    private static async Task OnInstallModules(Database db)
    {
        AppData.LoadAppData();
        var modules = AppData.Data.Modules?.Select(m => SysModule.Load(db.User, m)).OrderBy(m => m.ParentId).ThenBy(m => m.Sort).ToList();
        await db.DeleteAllAsync<SysModule>();
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
        var items = await db.QueryListAsync<SysModule>();
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