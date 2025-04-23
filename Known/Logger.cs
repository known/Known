namespace Known;

public partial class Logger
{
    private static void WriteLog(LogLevel type, LogTarget target, UserInfo user, string content)
    {
        if (Level > type)
            return;

        var log = new LogInfo
        {
            Type = type.ToString(),
            Target = target.ToString(),
            CreateBy = user?.Name ?? user?.UserName,
            CreateTime = DateTime.Now,
            Content = content
        };
        if (target == LogTarget.FrontEnd)
        {
            var scope = Config.ServiceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IAdminService>();
            service.AddWebLogAsync(log);
        }
        else
        {
            Logs.Add(log);
        }
    }
}