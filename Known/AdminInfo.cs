namespace Known;

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
    /// 取得或设置当前用户是否要提示修改密码。
    /// </summary>
    public bool IsChangePwd { get; set; }

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