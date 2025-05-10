namespace Known;

/// <summary>
/// 系统激活类型枚举。
/// </summary>
public enum ActiveType
{
    /// <summary>
    /// 系统。
    /// </summary>
    System,
    /// <summary>
    /// 组件。
    /// </summary>
    Component
}

/// <summary>
/// 系统激活信息类。
/// </summary>
public class ActiveInfo
{
    /// <summary>
    /// 取得或设置激活类型，默认系统。
    /// </summary>
    public ActiveType Type { get; set; }

    /// <summary>
    /// 取得或设置产品ID。
    /// </summary>
    [DisplayName("产品ID")]
    public string ProductId { get; set; }

    /// <summary>
    /// 取得或设置产品Key。
    /// </summary>
    [DisplayName("产品Key")]
    public string ProductKey { get; set; }

    /// <summary>
    /// 取得或设置操作是否成功。
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// 取得或设置操作成功或失败提示消息。
    /// </summary>
    public string Message { get; set; }
}

/// <summary>
/// 系统信息类。
/// </summary>
public class SystemInfo
{
    /// <summary>
    /// 取得或设置企业编码。
    /// </summary>
    public string CompNo { get; set; }

    /// <summary>
    /// 取得或设置企业名称。
    /// </summary>
    public string CompName { get; set; }

    /// <summary>
    /// 取得或设置系统名称。
    /// </summary>
    public string AppName { get; set; }

    /// <summary>
    /// 取得或设置产品ID。
    /// </summary>
    public string ProductId { get; set; }

    /// <summary>
    /// 取得或设置产品Key。
    /// </summary>
    public string ProductKey { get; set; }

    /// <summary>
    /// 取得或设置系统用户默认密码。
    /// </summary>
    public string UserDefaultPwd { get; set; }

    /// <summary>
    /// 取得或设置系统登录是否验证图片验证码。
    /// </summary>
    public bool IsLoginCaptcha { get; set; }
}

/// <summary>
/// 系统版本信息类。
/// </summary>
public class VersionInfo
{
    //private readonly Assembly assembly;

    /// <summary>
    /// 构造函数，创建一个系统版本信息类的实例。
    /// </summary>
    public VersionInfo() { }

    internal VersionInfo(Assembly assembly)
    {
        //this.assembly = assembly;
        if (assembly != null)
        {
            var version = assembly.GetName().Version;
            AppVersion = $"{Config.App.Id} V{version.Major}.{version.Minor}";
            SoftVersion = $"V{version.Major}.{version.Minor}.{version.Build}";
        }

        var version1 = typeof(VersionInfo).Assembly.GetName().Version;
        FrameVersion = $"Known V{version1.Major}.{version1.Minor}.{version1.Build}";
    }

    /// <summary>
    /// 取得或设置系统版本号。
    /// </summary>
    public string AppVersion { get; set; }

    /// <summary>
    /// 取得或设置软件版本号。
    /// </summary>
    public string SoftVersion { get; set; }

    /// <summary>
    /// 取得或设置框架版本号。
    /// </summary>
    public string FrameVersion { get; set; }

    /// <summary>
    /// 取得或设置系统编译时间。
    /// </summary>
    public DateTime BuildTime { get; set; }
}

/// <summary>
/// WebApi方法信息类。
/// </summary>
public class ApiMethodInfo
{
    /// <summary>
    /// 取得或设置方法ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置方法路由地址。
    /// </summary>
    [DisplayName("路由")]
    public string Route { get; set; }

    /// <summary>
    /// 取得或设置方法描述。
    /// </summary>
    [DisplayName("描述")]
    public string Description { get; set; }

    /// <summary>
    /// 取得或设置方法HTTP请求方式，默认方法名以Get开头的方法为GET请求，其他为POST请求。
    /// </summary>
    [Column(IsQueryAll = true)]
    [Category("GET,POST")]
    [DisplayName("HTTP请求")]
    public HttpMethod HttpMethod { get; set; }

    /// <summary>
    /// 取得或设置方法信息。
    /// </summary>
    public MethodInfo MethodInfo { get; set; }

    /// <summary>
    /// 取得或设置方法参数集合。
    /// </summary>
    public ParameterInfo[] Parameters { get; set; }
}