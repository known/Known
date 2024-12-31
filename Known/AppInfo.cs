namespace Known;

/// <summary>
/// 项目类型枚举。
/// </summary>
public enum AppType
{
    /// <summary>
    /// Web项目。
    /// </summary>
    Web,
    /// <summary>
    /// WebApi项目。
    /// </summary>
    WebApi,
    /// <summary>
    /// 桌面项目。
    /// </summary>
    Desktop
}

/// <summary>
/// 系统配置信息类。
/// </summary>
public class AppInfo
{
    /// <summary>
    /// 取得或设置系统ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置系统名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置系统类型，默认Web。
    /// </summary>
    public AppType Type { get; set; } = AppType.Web;

    /// <summary>
    /// 取得或设置系统入口程序集，用于获取软件版本号。
    /// </summary>
    public Assembly Assembly { get; set; }

    /// <summary>
    /// 取得或设置系统是否为多租户平台。
    /// </summary>
    public bool IsPlatform { get; set; }

    /// <summary>
    /// 取得或设置系统是否启用移动端页面。
    /// </summary>
    public bool IsMobile { get; set; }

    /// <summary>
    /// 取得或设置系统是否为Restful客户端。
    /// </summary>
    public bool IsClient { get; set; }

    /// <summary>
    /// 取得或设置系统主页顶部菜单是否显示字体大小切换，默认显示。
    /// </summary>
    public bool IsSize { get; set; } = true;

    /// <summary>
    /// 取得或设置系统主页顶部菜单是否显示语言切换，默认显示。
    /// </summary>
    public bool IsLanguage { get; set; } = true;

    /// <summary>
    /// 取得或设置系统主页顶部菜单是否显示主题切换，默认显示。
    /// </summary>
    public bool IsTheme { get; set; } = true;

    /// <summary>
    /// 取得或设置系统Web根目录。
    /// </summary>
    public string WebRoot { get; set; }

    /// <summary>
    /// 取得或设置系统内容根目录。
    /// </summary>
    public string ContentRoot { get; set; }

    /// <summary>
    /// 取得或设置系统附件上传位置，默认为根目录上级文件夹内的UploadFiles文件夹。
    /// </summary>
    public string UploadPath { get; set; }

    /// <summary>
    /// 取得或设置系统附件上传最大长度，默认50M。
    /// </summary>
    public long UploadMaxSize { get; set; } = 1024 * 1024 * 50;

    /// <summary>
    /// 取得或设置系统默认字体大小，默认为Default。
    /// </summary>
    public string DefaultSize { get; set; } = "Default";

    /// <summary>
    /// 取得或设置系统表格默认分页大小，默认10。
    /// </summary>
    public int DefaultPageSize { get; set; } = 10;

    /// <summary>
    /// 取得或设置系统JS脚本文件路径，该文件中的JS方法，可通过JSService的InvokeAppAsync和InvokeAppVoidAsync调用。
    /// </summary>
    public string JsPath { get; set; }
}