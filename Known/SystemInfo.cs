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
    /// 版本。
    /// </summary>
    Version,
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

    /// <summary>
    /// 取得或设置是否启用账号单一客户端登录。
    /// </summary>
    public bool IsLoginOne { get; set; }

    /// <summary>
    /// 取得或设置是否启用账号单一客户端登录。
    /// </summary>
    public string TipLoginOne { get; set; } = "您的账号在其他电脑登录，您已被迫退出。";

    /// <summary>
    /// 取得或设置是否启用默认密码修改提醒。
    /// </summary>
    public bool IsChangePwd { get; set; }

    /// <summary>
    /// 取得或设置默认密码修改提醒提示内容。
    /// </summary>
    public string TipChangePwd { get; set; } = "您当前使用默认密码登录，为了安全，请及时修改。";

    /// <summary>
    /// 取得或设置密码长度。
    /// </summary>
    public int? PwdLength { get; set; } = 6;

    /// <summary>
    /// 取得或设置密码复杂度，默认None不限制，包含None、Low、Middle、High四种复杂度。
    /// </summary>
    public string PwdComplexity { get; set; } = nameof(PasswordComplexity.None);

    /// <summary>
    /// 取得或设置系统是否启用水印。
    /// </summary>
    public bool IsWatermark { get; set; }

    /// <summary>
    /// 取得或设置账号水印格式。
    /// </summary>
    public string Watermark { get; set; }

    /// <summary>
    /// 取得或设置允许附件大小，单位M。
    /// </summary>
    public int? MaxFileSize { get; set; }
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