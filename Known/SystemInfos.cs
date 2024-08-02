namespace Known;

public class InstallInfo
{
    public string CompNo { get; set; }
    public string CompName { get; set; }
    public string AppName { get; set; }

    public string ProductId { get; set; }
    public string ProductKey { get; set; }

    public string AdminName { get; set; }
    public string AdminPassword { get; set; }
    public string Password1 { get; set; }

    public List<DatabaseInfo> Databases { get; set; }
}

public class DatabaseInfo
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string ConnectionString { get; set; }
}

public class SystemInfo
{
    public string CompNo { get; set; }
    public string CompName { get; set; }
    public string AppName { get; set; }
    public string ProductId { get; set; }
    public string ProductKey { get; set; }
    public string UserDefaultPwd { get; set; }
    public bool IsLoginCaptcha { get; set; }
}

public class SystemDataInfo
{
    public SystemInfo System { get; set; }
    public double RunTime { get; set; }
    public VersionInfo Version { get; set; }
}