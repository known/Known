namespace Known.Shared;

public static class AppConfig
{
    public const string Branch = "Known";
    public const string SubTitle = "基于Blazor的企业级快速开发框架";

    public static void AddShared(this IServiceCollection services)
    {
        Config.AddModule(typeof(AppConfig).Assembly);
    }
}