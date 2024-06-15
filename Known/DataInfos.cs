namespace Known;

public class AdminInfo
{
    public string AppName { get; set; }
    public int MessageCount { get; set; }
    public SettingInfo UserSetting { get; set; }
    public List<MenuInfo> UserMenus { get; set; }
    public List<CodeInfo> Codes { get; set; }
}

public class AutoInfo<TData>
{
    public string PageId { get; set; }
    public TData Data { get; set; }
}

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

public class FileDataInfo
{
    public string FileName { get; set; }
    public byte[] Data { get; set; }
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