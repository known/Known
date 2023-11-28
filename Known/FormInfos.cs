using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Known.Extensions;
using Known.Helpers;

namespace Known;

public class LoginFormInfo
{
    [Required(ErrorMessage = "请输入用户名！")]
    public string UserName { get; set; }
    [Required(ErrorMessage = "请输入密码！")]
    public string Password { get; set; }
    public bool Remember { get; set; }
    public bool IsMobile { get; set; }
    public string IPAddress { get; set; }
}

public class PwdFormInfo
{
    [DisplayName("原密码")]
    [Required(ErrorMessage = "请输入原密码！")]
    public string OldPwd { get; set; }
    [DisplayName("新密码")]
    [Required(ErrorMessage = "请输入新密码！")]
    public string NewPwd { get; set; }
    [DisplayName("确认密码")]
    [Required(ErrorMessage = "请输入确认密码！")]
    public string NewPwd1 { get; set; }
}

public class UploadInfo<TModel>(TModel model)
{
    public TModel Model { get; } = model;
    public Dictionary<string, List<IAttachFile>> Files { get; } = [];
}

public class FileFormInfo
{
    public string Category { get; set; }
    public string BizId { get; set; }
    public string BizName { get; set; }
    public string BizType { get; set; }
    public string BizPath { get; set; }
}

public class ImportFormInfo : FileFormInfo
{
    public string Name { get; set; }
    public bool IsAsync { get; set; }
    public bool IsFinished { get; set; } = true;
    public string Message { get; set; }
    public string Error { get; set; }

    public static List<string> GetImportColumns(string modelType)
    {
        var baseProperties = typeof(EntityBase).GetProperties();
        var attrs = TypeHelper.GetColumnAttributes(modelType);
        return attrs.Where(a => !baseProperties.Any(p => p.Name == a.Property.Name))
                    .Select(a => a.Property.DisplayName())
                    .ToList();
    }
}