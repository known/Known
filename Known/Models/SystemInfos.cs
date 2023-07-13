namespace Known.Models;

public class InstallInfo
{
    public string CompNo { get; set; }
    public string CompName { get; set; }
    public string AppName { get; set; }
    public string ProductId { get; set; }
    public string ProductKey { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Password1 { get; set; }
}

public class CheckInfo
{
    public bool IsCheckKey { get; set; }
    public bool IsInstalled { get; set; } = true;
    public InstallInfo Install { get; set; }
}

public class SystemInfo
{
    public string CompNo { get; set; }
    public string CompName { get; set; }
    public string AppName { get; set; }
    public string ProductId { get; set; }
    public string ProductKey { get; set; }
    public string UserDefaultPwd { get; set; }
    public string Copyright { get; set; }
    public string SoftTerms { get; set; }
}