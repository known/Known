namespace Known;

public class LoginFormInfo
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string PhoneNo { get; set; }
    public string PhoneCode { get; set; }
    public string Captcha { get; set; }
    public string Station { get; set; }
    public bool Remember { get; set; }
    public bool IsMobile { get; set; }
    public string IPAddress { get; set; }
    public string TabKey { get; set; }
}

public class PwdFormInfo
{
    [Form(Type = "Password"), Required]
    public string OldPwd { get; set; }

    [Form(Type = "Password"), Required]
    public string NewPwd { get; set; }

    [Form(Type = "Password"), Required]
    public string NewPwd1 { get; set; }
}

public class UploadInfo<TModel>(TModel model)
{
    public TModel Model { get; } = model;
    public Dictionary<string, List<IAttachFile>> Files { get; } = [];

    public bool HasFile(string key)
    {
        if (Files == null)
            return false;

        if (!Files.TryGetValue(key, out List<IAttachFile> value))
            return false;

        return value.Count > 0;
    }
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
        var baseProperties = TypeHelper.Properties(typeof(EntityBase));
        var attrs = TypeHelper.GetColumnAttributes(modelType);
        return attrs.Where(a => !baseProperties.Any(p => p.Name == a.Property.Name))
                    .Select(a => a.Property.DisplayName())
                    .ToList();
    }
}