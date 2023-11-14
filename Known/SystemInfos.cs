using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Known;

public class InstallInfo
{
    public bool IsInstalled { get; set; }
    
    [DisplayName("企业编码")]
    [Required(ErrorMessage = "企业编码不能为空！")]
    public string CompNo { get; set; }
    
    [DisplayName("企业名称")]
    [Required(ErrorMessage = "企业名称不能为空！")]
    public string CompName { get; set; }

    [DisplayName("系统名称")]
    [Required(ErrorMessage = "系统名称不能为空！")]
    public string AppName { get; set; }

    [DisplayName("产品ID")]
    [Required(ErrorMessage = "产品ID不能为空！")]
    public string ProductId { get; set; }

    [DisplayName("产品密钥")]
    [Required(ErrorMessage = "产品密钥不能为空！")]
    public string ProductKey { get; set; }

    [DisplayName("管理员账号")]
    [Required(ErrorMessage = "管理员账号不能为空！")]
    public string UserName { get; set; }

    [DisplayName("管理员密码")]
    [Required(ErrorMessage = "管理员密码不能为空！")]
    public string Password { get; set; }

    [DisplayName("确认密码")]
    [Required(ErrorMessage = "确认密码不能为空！")]
    public string Password1 { get; set; }
}

public class SystemInfo
{
    [DisplayName("企业编码")]
    public string CompNo { get; set; }

    [DisplayName("企业名称")]
    public string CompName { get; set; }

    [DisplayName("系统名称")]
    public string AppName { get; set; }

    [DisplayName("产品ID")]
    public string ProductId { get; set; }

    [DisplayName("产品密钥")]
    public string ProductKey { get; set; }

    [DisplayName("默认密码")]
    public string UserDefaultPwd { get; set; }

    [DisplayName("版权信息")]
    public string Copyright { get; set; }

    [DisplayName("软件许可")]
    public string SoftTerms { get; set; }
}