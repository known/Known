namespace Known.Core;

public sealed class KCConfig
{
    private KCConfig() { }

    public static AppInfo App { get; set; } = new AppInfo();
    public static string RootPath => AppDomain.CurrentDomain.BaseDirectory;
    public static string WebRoot { get; set; }
    public static string ContentRoot { get; set; }
    public static bool IsDevelopment { get; set; }

    public static void AddWebPlatform()
    {
        Container.Register<IPlatform, WebPlatform>();
    }

    public static string GetUploadPath()
    {
        var app = App;
        var uploadPath = app.UploadPath;
        if (string.IsNullOrEmpty(uploadPath))
            uploadPath = Path.Combine(ContentRoot, "UploadFiles");

        if (!Directory.Exists(uploadPath))
            Directory.CreateDirectory(uploadPath);

        return uploadPath;
    }

    public static string GetUploadPath(string filePath)
    {
        var uploadPath = GetUploadPath();
        return Path.Combine(uploadPath, filePath);
    }
}

public class AppInfo
{
    public string CompNo { get; set; }
    public string CompName { get; set; }
    public string UploadPath { get; set; }
    public string Templates { get; set; }
    public string ExportTemplate { get; set; }
    public string ExportPath { get; set; }
    public Dictionary<string, object> Params { get; set; }
    public List<ConnectionInfo> Connections { get; set; }

    public ConnectionInfo GetConnection(string name)
    {
        if (Connections == null || Connections.Count == 0)
            return null;

        return Connections.FirstOrDefault(c => c.Name == name);
    }

    public T Param<T>(string key, T defaultValue = default)
    {
        if (Params == null || Params.Count == 0)
            return defaultValue;

        if (!Params.ContainsKey(key))
            return defaultValue;

        var value = Params[key];
        if (typeof(T).IsClass)
            return Utils.MapTo<T>(value);

        return Utils.ConvertTo(Params[key], defaultValue);
    }
}

public class ConnectionInfo
{
    public string Name { get; set; }
    public string ProviderName { get; set; }
    public string ProviderType { get; set; }
    public string ConnectionString { get; set; }
}