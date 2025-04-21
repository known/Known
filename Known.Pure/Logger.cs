namespace Known;

public partial class Logger
{
    private static void WriteLog(LogLevel type, LogTarget target, UserInfo user, string content)
    {
        if (Level > type)
            return;

        var log = new LogInfo { Type = type.ToString(), Target = target.ToString(), CreateBy = user?.UserName, CreateTime = DateTime.Now, Content = content };
        Logs.Add(log);
    }
}