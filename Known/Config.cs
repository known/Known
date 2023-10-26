namespace Known;

public sealed class Config
{
    private Config() { }

    static Config()
    {
        AppId = "KIMS";
        AppName = "Known信息管理系统";
        Version = "1.0";
        var version = typeof(Config).Assembly.GetName().Version;
        FrameVersion = $"Known V{version.Major}.{version.Minor}.{version.Build}";
    }

    public static string DateFormat => "yyyy-MM-dd";
    public static string DateTimeFormat => "yyyy-MM-dd HH:mm:ss";
    public static string AppVersion => $"PM-{AppId} {Version}";
    public static string AppId { get; set; }
    public static string AppName { get; set; }
    public static string Version { get; private set; }
    public static string SoftVersion { get; private set; }
    public static string FrameVersion { get; private set; }
    public static Assembly AppAssembly { get; private set; }
    public static bool IsPlatform { get; set; }
    public static bool IsWebApi { get; set; } = true;

    public static AppInfo App { get; set; } = new AppInfo();
    public static List<Assembly> Modules { get; } = [];
    public static string RootPath => AppDomain.CurrentDomain.BaseDirectory;
    public static string WebRoot { get; set; }
    public static string ContentRoot { get; set; }
    public static bool IsDevelopment { get; set; }

    public static void SetAppAssembly(Assembly assembly)
    {
        AppAssembly = assembly;
        var version = assembly.GetName().Version;
        SoftVersion = version.ToString();
        Version = $"{version.Major}.{version.Minor}";
    }

    public static string GetSysVersion(Assembly assembly)
    {
        var version = assembly.GetName().Version;
        var date = new DateTime(2000, 1, 1).AddDays(version.Build).AddSeconds(version.Revision * 2);
        return $"{Version}.{date:yyMMdd}";
    }

    internal static string GetUploadPath(bool isWeb = false)
    {
        if (isWeb)
        {
            var path = WebRoot ?? ContentRoot;
            var filePath = Path.Combine(path, "Files");
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            return filePath;
        }

        var app = App;
        var uploadPath = app.UploadPath;
        if (string.IsNullOrEmpty(uploadPath))
            uploadPath = Path.Combine(ContentRoot, "UploadFiles");

        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        return uploadPath;
    }

    public static string GetUploadPath(string filePath, bool isWeb = false)
    {
        var path = GetUploadPath(isWeb);
        return Path.Combine(path, filePath);
    }
}