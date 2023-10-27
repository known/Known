namespace Known;

public class InstallInfo
{
    public bool IsInstalled { get; set; }
    public string CompNo { get; set; }
    public string CompName { get; set; }
    public string AppName { get; set; }
    public string ProductId { get; set; }
    public string ProductKey { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Password1 { get; set; }
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