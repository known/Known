namespace Known;

public static class AdmExtension
{
    public static void AddKnownAdminClient(this IServiceCollection services)
    {
        services.AddKnownAdmin();
    }

    public static void AddKnownAdminCore(this IServiceCollection services)
    {
        services.AddKnownAdmin();

        //CoreConfig.OnInstallModules = OnInstallModules;
        //CoreConfig.OnInitialModules = OnInitialModules;
    }

    private static void AddKnownAdmin(this IServiceCollection services)
    {
        var assembly = typeof(AdmExtension).Assembly;
        Config.AddModule(assembly);

        // 配置UI
        //UIConfig.TopNavType = typeof(KTopNavbar);
        //UIConfig.ModulePageType = typeof(ModuleList);
        //UIConfig.EnableEdit = false;
        // 添加样式
        KStyleSheet.AddStyle("_content/Known.Admin/css/web.css");
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