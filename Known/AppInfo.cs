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
/// ID生成器类型枚举。
/// </summary>
public enum NextIdType
{
    /// <summary>
    /// Guid。
    /// </summary>
    Guid,
    /// <summary>
    /// 雪花ID。
    /// </summary>
    Snowflake
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
    /// 取得或设置ID生成器类型。
    /// </summary>
    public NextIdType NextIdType { get; set; }

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
    public int UploadMaxSize { get; set; } = 50;

    /// <summary>
    /// 取得或设置系统默认字体大小，默认为Default。
    /// </summary>
    public string DefaultSize { get; set; } = "Default";

    /// <summary>
    /// 取得或设置系统表格默认分页大小，默认20。
    /// </summary>
    public int DefaultPageSize { get; set; } = 20;

    /// <summary>
    /// 取得或设置系统JS脚本文件路径，该文件中的JS方法，可通过JSService的InvokeAppAsync和InvokeAppVoidAsync调用。
    /// </summary>
    public string JsPath { get; set; }

    /// <summary>
    /// 取得或设置Web日志保留天数，默认7天。
    /// </summary>
    public int WebLogDays { get; set; } = 7;

    /// <summary>
    /// 取得或设置登录用户过期时长，默认30分钟。
    /// </summary>
    public TimeSpan AuthExpired { get; set; } = TimeSpan.FromMinutes(30);

    /// <summary>
    /// 取得系统WebApi配置信息。
    /// </summary>
    public WebApiOption WebApi { get; } = new WebApiOption();

    /// <summary>
    /// 取得或设置代码配置信息。
    /// </summary>
    public CodeConfigInfo Code { get; set; }

    /// <summary>
    /// 取得或设置微信配置信息。
    /// </summary>
    public WeixinConfigInfo Weixin { get; set; }

    /// <summary>
    /// 取得或设置邮件配置信息。
    /// </summary>
    public EmailConfigInfo Email { get; set; }

    /// <summary>
    /// 取得或设置数据库访问配置选项委托。
    /// </summary>
    public Action<DatabaseOption> Database { get; set; }

    /// <summary>
    /// 取得或设置系统因未处理异常导致退出的委托，可用于发送报警。
    /// </summary>
    public Action<Exception> OnExit { get; set; }
}

/// <summary>
/// WebApi配置信息类。
/// </summary>
public class WebApiOption
{
    /// <summary>
    /// 取得或设置系统WebApi是否加密传输数据。
    /// </summary>
    public bool IsEncrypt { get; set; }

    /// <summary>
    /// 取得或设置系统WebApi加密传输数据的密码。
    /// </summary>
    public string DataPassword { get; set; }
}

/// <summary>
/// 代码配置信息类。
/// </summary>
public class CodeConfigInfo
{
    /// <summary>
    /// 取得或设置前实体类路径。
    /// </summary>
    public string EntityPath { get; set; }

    /// <summary>
    /// 取得或设置前信息类路径。
    /// </summary>
    public string ModelPath { get; set; }

    /// <summary>
    /// 取得或设置前端页面路径。
    /// </summary>
    public string PagePath { get; set; }

    /// <summary>
    /// 取得或设置前端表单路径。
    /// </summary>
    public string FormPath { get; set; }

    /// <summary>
    /// 取得或设置服务接口路径。
    /// </summary>
    public string ServiceIPath { get; set; }

    /// <summary>
    /// 取得或设置服务实现类路径。
    /// </summary>
    public string ServicePath { get; set; }
}