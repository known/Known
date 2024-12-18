using Known.Platforms;

namespace Known;

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

        // 注入平台服务
        services.AddScoped<IPlatformService, PlatformService>();
        services.AddScoped<IAutoService, AutoService>();
        services.AddScoped(typeof(IEntityService<>), typeof(EntityService<>));

        // 注入服务
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ISystemService, SystemService>();
        services.AddScoped<IInstallService, InstallService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IModuleService, ModuleService>();
        services.AddScoped<IDictionaryService, DictionaryService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<ILogService, LogService>();
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IFlowService, FlowService>();
        services.AddScoped<IImportService, ImportService>();

        // 添加模块
        Config.AddModule(typeof(AdminExtension).Assembly);

        // 配置UI
        var routes = "/install,/login,/profile,/profile/user,/profile/password,/app,/app/mine";
        UIConfig.IgnoreRoutes.AddRange(routes.Split(','));
        UIConfig.AdminBody = (b, d) => b.Component<KAuthPanel>().Set(c => c.ChildContent, d).Build();
        UIConfig.ImportForm = BuildImportForm;
        UIConfig.UserProfileType = typeof(UserProfileInfo);
        UIConfig.UserTabs["MyProfile"] = typeof(UserEditForm);
        UIConfig.UserTabs["SecuritySetting"] = typeof(PasswordEditForm);
        UIConfig.DevelopTabs["Menu.SysModuleList"] = typeof(ModuleList);
        UIConfig.DevelopTabs["WebApi"] = typeof(WebApiList);
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
        WeixinApi.Initialize(AdminOption.Instance.Weixin);

        ModuleDB.IsAppData = false;
        AdminOption.Instance.AddAssembly(typeof(AdminExtension).Assembly);

        // 注入EFCore模型
        DbConfig.Models.Add<SysCompany>(x => x.Id);
        DbConfig.Models.Add<SysDictionary>(x => x.Id);
        DbConfig.Models.Add<SysFile>(x => x.Id);
        DbConfig.Models.Add<SysFlow>(x => x.Id);
        DbConfig.Models.Add<SysFlowLog>(x => x.Id);
        DbConfig.Models.Add<SysLog>(x => x.Id);
        DbConfig.Models.Add<SysMessage>(x => x.Id);
        DbConfig.Models.Add<SysModule>(x => x.Id);
        DbConfig.Models.Add<SysOrganization>(x => x.Id);
        DbConfig.Models.Add<SysRole>(x => x.Id);
        DbConfig.Models.Add<SysSetting>(x => x.Id);
        DbConfig.Models.Add<SysTask>(x => x.Id);
        DbConfig.Models.Add<SysUser>(x => x.Id);
        DbConfig.Models.Add<SysWeixin>(x => x.Id);
        DbConfig.Models.Add<SysConfig>(x => new { x.AppId, x.ConfigKey });
        DbConfig.Models.Add<SysRoleModule>(x => new { x.RoleId, x.ModuleId });
        DbConfig.Models.Add<SysUserRole>(x => new { x.UserId, x.RoleId });

        // 注入后台任务
        TaskHelper.OnPendingTask = GetPendingTaskAsync;
        TaskHelper.OnSaveTask = SaveTaskAsync;
    }

    /// <summary>
    /// 添加Known框架简易微信功能模块前端。
    /// </summary>
    /// <param name="services">服务集合。</param>
    public static void AddKnownWeixin(this IServiceCollection services)
    {
        services.AddScoped<IWeixinService, WeixinService>();
        // 配置UI
        AdminConfig.SystemTabs["WeChatSetting"] = b => b.Component<WeChatSetting>().Build();
    }

    private static async Task<TaskInfo> GetPendingTaskAsync(Database db, string type)
    {
        var info = await db.Query<SysTask>().Where(d => d.Status == TaskJobStatus.Pending && d.Type == type)
                           .OrderBy(d => d.CreateTime).FirstAsync<TaskInfo>();
        if (info != null)
        {
            db.User = await db.GetUserAsync(info.CreateBy);
            info.File = await db.Query<SysFile>().Where(d => d.Id == info.Target).FirstAsync<AttachInfo>();
        }
        return info;
    }

    private static Task SaveTaskAsync(Database db, TaskInfo info)
    {
        return db.SaveTaskAsync(info);
    }

    private static void BuildImportForm(RenderTreeBuilder builder, ImportInfo info)
    {
        builder.Component<Importer>().Set(c => c.Info, info).Build();
    }
}