namespace Known;

/// <summary>
/// 动态组件信息类。
/// </summary>
public class ComponentInfo
{
    /// <summary>
    /// 取得或设置组件排序。
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 取得或设置组件类型。
    /// </summary>
    public Type Type { get; set; }

    /// <summary>
    /// 取得或设置组件参数。
    /// </summary>
    public Dictionary<string, object> Parameters { get; set; }
}

/// <summary>
/// 后台管理主页数据交互信息类。
/// </summary>
public class AdminInfo
{
    /// <summary>
    /// 取得或设置系统名称。
    /// </summary>
    public string AppName { get; set; }

    /// <summary>
    /// 取得或设置当前系统数据库类型。
    /// </summary>
    public DatabaseType DatabaseType { get; set; }

    /// <summary>
    /// 取得或设置系统相关配置信息。
    /// </summary>
    public Dictionary<string, object> Settings { get; set; } = [];

    /// <summary>
    /// 取得或设置用户设置信息。
    /// </summary>
    public UserSettingInfo UserSetting { get; set; }

    /// <summary>
    /// 取得或设置当前用户模块表格设置信息列表，如：设置模块表格的显隐和宽度。
    /// </summary>
    public Dictionary<string, List<TableSettingInfo>> UserTableSettings { get; set; }

    /// <summary>
    /// 取得或设置当前用户权限菜单列表。
    /// </summary>
    public List<MenuInfo> UserMenus { get; set; }

    /// <summary>
    /// 取得或设置系统数据字典和代码表信息列表，用于前后端分离时，缓存在前端。
    /// </summary>
    public List<CodeInfo> Codes { get; set; }

    /// <summary>
    /// 取得或设置系统按钮信息列表。
    /// </summary>
    public List<ActionInfo> Actions { get; set; }
}

/// <summary>
/// 验证码选项类。
/// </summary>
public class CaptchaOption
{
    /// <summary>
    /// 取得或设置验证码图片URL。
    /// </summary>
    public string ImgUrl { get; set; }

    /// <summary>
    /// 取得或设置短信验证码倒计时长度。
    /// </summary>
    public int SMSCount { get; set; }

    /// <summary>
    /// 取得或设置短信验证码验证委托。
    /// </summary>
    public Func<Result> SMSValidate { get; set; }

    /// <summary>
    /// 取得或设置短信验证码发送委托。
    /// </summary>
    public Action SMSAction { get; set; }
}