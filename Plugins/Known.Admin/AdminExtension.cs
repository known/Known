﻿namespace Known;

/// <summary>
/// 依赖注入扩展类。
/// </summary>
public static class AdminExtension
{
    /// <summary>
    /// 添加Known框架后台管理模块前端。
    /// </summary>
    /// <param name="services">服务集合。</param>
    public static void AddKnownAdmin(this IServiceCollection services)
    {
        services.AddSingleton<ICodeGenerator, CodeGenerator>();

        // 注入服务
        services.AddScoped<IModuleService, ModuleService>();

        // 添加模块
        Config.AddModule(typeof(AdminExtension).Assembly);

        // 配置UI
        UIConfig.TopNavType = typeof(KTopNavbar);
        UIConfig.EnableEdit = false;

        // 添加样式
        KStyleSheet.AddStyle("_content/Known.Admin/css/web.css");
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

        // 注入EFCore模型
        DbConfig.Models.Add<SysMessage>(x => x.Id);
        DbConfig.Models.Add<SysModule>(x => x.Id);

        CoreConfig.OnInstallModules = OnInstallModules;
        CoreConfig.OnInitialModules = OnInitialModules;
    }

    private static async Task OnInstallModules(Database db)
    {
        var modules = ModuleHelper.GetModules();
        await db.DeleteAllAsync<SysModule>();
        await db.InsertAsync(modules);
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
            AppData.Initialize(modules);
        }
        return modules;
    }
}