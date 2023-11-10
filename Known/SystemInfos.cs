using System.ComponentModel.DataAnnotations;

namespace Known;

public class InstallInfo
{
    public bool IsInstalled { get; set; }
    [Required] public string CompNo { get; set; }
    [Required] public string CompName { get; set; }
    [Required] public string AppName { get; set; }
    [Required] public string ProductId { get; set; }
    [Required] public string ProductKey { get; set; }
    [Required] public string UserName { get; set; }
    [Required] public string Password { get; set; }
    [Required] public string Password1 { get; set; }
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