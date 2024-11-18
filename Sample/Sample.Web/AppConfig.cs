namespace Sample.Web;

public static class AppConfig
{
    public const string AppId = "KIMS";
    public const string AppName = "Known信息管理系统";

    public static void AddApplication(this WebApplicationBuilder builder)
    {
        builder.Services.AddKnown(info =>
        {
            info.Id = AppId;
            info.Name = AppName;
        });
    }

    public static void UseApplication(this WebApplication app)
    {
    }
}