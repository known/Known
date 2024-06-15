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
    public string PageId { get; set; }
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
        var columns = new List<string>();
        var baseProperties = TypeHelper.Properties(typeof(EntityBase));
        var type = Type.GetType(modelType);
        var properties = TypeHelper.Properties(type);
        foreach (var item in properties)
        {
            if (item.GetGetMethod().IsVirtual || baseProperties.Any(p => p.Name == item.Name))
                continue;

            var name = item.DisplayName();
            if (!string.IsNullOrWhiteSpace(name))
                columns.Add(name);
        }
        return columns;
    }
}