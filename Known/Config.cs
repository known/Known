namespace Known;

public sealed class Config
{
    private Config() { }

    static Config()
    {
        AppId = "DEV";
        AppName = "普漫快速开发平台";
        Version = "1.0";
    }

    public static string DateFormat => "yyyy-MM-dd";
    public static string DateTimeFormat => "yyyy-MM-dd HH:mm:ss";
    public static string AppVersion => $"PM-{AppId} {Version}";
    public static string AppId { get; set; }
    public static string AppName { get; set; }
    public static string Version { get; set; }
    public static string SysVersion { get; set; }
    public static Assembly AppAssembly { get; set; }

    public static string GetSysVersion(Assembly assembly)
    {
        //var path = assembly.Location;
        //var file = new FileInfo(path);
        //return $"{Version}.{file.LastWriteTime:yyMMdd}";

        var version = assembly.GetName().Version;
        var date = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
        return $"{Version}.{date:yyMMdd}";
    }
}