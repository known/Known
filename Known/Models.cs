namespace Known;

public class CountInfo
{
    public string Field1 { get; set; }
    public string Field2 { get; set; }
    public string Field3 { get; set; }
    public int TotalCount { get; set; }
}

public class ChartDataInfo
{
    public string Name { get; set; }
    public Dictionary<string, object> Series { get; set; }
}

public class AdminInfo
{
    public string AppName { get; set; }
    public int MessageCount { get; set; }
    public UserSetting UserSetting { get; set; }
    public List<MenuInfo> UserMenus { get; set; }
    //public List<CodeInfo> Codes { get; set; }
}

public class FileUrlInfo
{
    public string FileName { get; set; }
    public string ThumbnailUrl { get; set; }
    public string OriginalUrl { get; set; }
}

public class TaskSummaryInfo
{
    public string Status { get; set; }
    public string Message { get; set; }
}

public class ConfigInfo
{
    public string Key { get; set; }
    public object Value { get; set; }
}

//public class CodeName
//{
//    public CodeName() { }

//    public CodeName(string code, string name)
//    {
//        Code = code;
//        Name = name;
//    }

//    public string Code { get; set; }
//    public string Name { get; set; }
//}