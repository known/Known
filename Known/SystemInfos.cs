namespace Known;

public class InstallInfo
{
    [Form, Required]
    public string CompNo { get; set; }

    [Form, Required]
    public string CompName { get; set; }

    [Form, Required]
    public string AppName { get; set; }

    [Required]
    public string ProductId { get; set; }

    [Required]
    public string ProductKey { get; set; }

    [Form(ReadOnly = true), Required]
    public string AdminName { get; set; }

    [Form(Type = "Password"), Required]
    public string AdminPassword { get; set; }

    [Form(Type = "Password"), Required]
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
    public bool IsLoginCaptcha { get; set; }
}

public class SystemDataInfo
{
    public SystemInfo System { get; set; }
    public double RunTime { get; set; }
    public VersionInfo Version { get; set; }
}