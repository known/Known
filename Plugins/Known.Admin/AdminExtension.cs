namespace Known;

/// <summary>
/// 依赖注入扩展类。
/// </summary>
public static class AdminExtension
{
    internal static readonly AdminOption Option = new();

    /// <summary>
    /// 添加Known框架后台管理模块前端。
    /// </summary>
    /// <param name="services">服务集合。</param>
    public static void AddKnownAdmin(this IServiceCollection services)
    {
        // 注入服务
        services.AddSingleton<IAdminService, AdminService>();
        services.AddScoped<IModuleService, ModuleService>();
        services.AddScoped<IDictionaryService, DictionaryService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<ILogService, LogService>();
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IFlowService, FlowService>();

        // 添加配置
        Config.AdminTasks["Admin"] = SetAdminInfoAsync;
        Config.AddModule(typeof(AdminExtension).Assembly);

        // 配置UI
        UIConfig.DevelopTabs["Menu.SysModuleList"] = b => b.Component<SysModuleList>().Build();

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
        action?.Invoke(Option);
        WeixinApi.Initialize(Option.Weixin);

        // 注入后台任务
        TaskHelper.OnPendingTask = GetPendingTaskAsync;
        TaskHelper.OnSaveTask = SaveTaskAsync;

        // 映射数据表
        DbConfig.MapEntity<ModuleInfo, SysModule>();
        DbConfig.MapEntity<UserInfo, SysUser>();
        DbConfig.MapEntity<AttachInfo, SysFile>();
        DbConfig.MapEntity<TaskInfo, SysTask>();
        DbConfig.MapEntity<SettingInfo, SysSetting>();
    }

    /// <summary>
    /// 添加Known框架简易微信功能模块前端。
    /// </summary>
    /// <param name="services">服务集合。</param>
    public static void AddKnownWeixin(this IServiceCollection services)
    {
        services.AddScoped<IWeixinService, WeixinService>();
        // 配置UI
        UIConfig.SystemTabs["WeChatSetting"] = b => b.Component<WeChatSetting>().Build();
    }

    private static async Task<TaskInfo> GetPendingTaskAsync(Database db, string type)
    {
        var info = await db.Query<TaskInfo>().Where(d => d.Status == SysTaskStatus.Pending && d.Type == type)
                           .OrderBy(d => d.CreateTime).FirstAsync();
        if (info != null)
            info.File = await db.QueryAsync<AttachInfo>(d => d.Id == info.Target);
        return info;
    }

    private static Task SaveTaskAsync(Database db, TaskInfo info)
    {
        return db.SaveTaskAsync(info);
    }

    private static async Task SetAdminInfoAsync(Database db, AdminInfo info)
    {
        info.MessageCount = await db.CountAsync<SysMessage>(d => d.UserId == db.User.UserName && d.Status == Constant.UMStatusUnread);
        info.Codes = await DictionaryService.GetDictionariesAsync(db);
    }
}