namespace Known.Admin;

/// <summary>
/// 依赖注入扩展类。
/// </summary>
public static class Extension
{
    private static readonly AdminOption option = new();

    /// <summary>
    /// 添加Known框架后台权限管理模块。
    /// </summary>
    /// <param name="services">服务集合。</param>
    /// <param name="action">配置选项委托。</param>
    public static void AddKnownAdmin(this IServiceCollection services, Action<AdminOption> action = null)
    {
        action?.Invoke(option);
        var assembly = typeof(Extension).Assembly;

        services.AddScoped<IPlatformService, PlatformService>();
        services.AddScoped<IDictionaryService, DictionaryService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<ILogService, LogService>();
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<ISysUserService, SysUserService>();

        Config.AdminTasks["Admin"] = async (db, info) =>
        {
            info.MessageCount = await db.CountAsync<SysMessage>(d => d.UserId == db.User.UserName && d.Status == Constant.UMStatusUnread);
            info.Codes = await DictionaryService.GetDictionariesAsync(db);
        };
        Config.AddModule(assembly);
        KStyleSheet.AddStyle("_content/Known.Admin/css/web.css");
    }
}