namespace Known;

public class LoginFormInfo
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public string ClientId { get; set; }
    public bool IsForce { get; set; }
    public bool IsMobile { get; set; }
    public string IPAddress { get; set; }
}

public class PwdFormInfo
{
    public string OldPwd { get; set; }
    public string NewPwd { get; set; }
    public string NewPwd1 { get; set; }
}

public class RoleFormInfo
{
    public dynamic Model { get; set; }
    public List<MenuInfo> Menus { get; set; }
    public List<string> MenuIds { get; set; }
}

public class SettingFormInfo
{
    public string Type { get; set; }
    public string Name { get; set; }
    public string Data { get; set; }
}

public class UploadInfo
{
    public string Name { get; set; }
    public string Type { get; set; }
    public byte[] Data { get; set; }
}

public class UploadFormInfo
{
    public UploadFormInfo() { }

    public UploadFormInfo(string model)
    {
        Model = Utils.ToDynamic(model);
        Files = new Dictionary<string, List<IAttachFile>>();
    }

    public dynamic Model { get; set; }
    public Dictionary<string, List<IAttachFile>> Files { get; set; }
}

public class FileFormInfo
{
    public string Category { get; set; }
    public string BizId { get; set; }
    public string BizName { get; set; }
    public string BizType { get; set; }
    public string BizPath { get; set; }
    public string IsThumb { get; set; }
}

public class ImportFormInfo : FileFormInfo
{
    public string Type { get; set; }
    public bool IsAsync { get; set; }
    public bool IsFinished { get; set; } = true;
    public string Message { get; set; }
    public string Error { get; set; }
    public List<string> Columns { get; set; }

    public List<string> GetImportColumns()
    {
        if (Columns != null && Columns.Count > 0)
            return Columns;

        return GetImportColumns(Type);
    }

    public static List<string> GetImportColumns(string modelType)
    {
        var baseProperties = typeof(EntityBase).GetProperties();
        var attrs = TypeHelper.GetColumnAttributes(modelType);
        return attrs.Where(a => !baseProperties.Any(p => p.Name == a.Property.Name)).Select(a => a.Description).ToList();
    }
}