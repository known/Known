﻿namespace Known;

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
    /// 取得或设置当前用户未读消息数量（暂未使用）。
    /// </summary>
    public int MessageCount { get; set; }

    /// <summary>
    /// 取得或设置用户设置信息。
    /// </summary>
    public UserSettingInfo UserSetting {  get; set; }

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
}

/// <summary>
/// 系统安装信息类。
/// </summary>
public class InstallInfo
{
    /// <summary>
    /// 取得或设置系统是否已经安装。
    /// </summary>
    public bool IsInstalled { get; set; }

    /// <summary>
    /// 取得或设置是否需要配置数据库连接。
    /// </summary>
    public bool IsDatabase { get; set; }

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
    /// 取得或设置管理员用户名。
    /// </summary>
    public string AdminName { get; set; }

    /// <summary>
    /// 取得或设置管理员密码。
    /// </summary>
    public string AdminPassword { get; set; }

    /// <summary>
    /// 取得或设置管理员确认密码。
    /// </summary>
    public string Password1 { get; set; }

    /// <summary>
    /// 取得或设置数据库信息列表。
    /// </summary>
    public List<DatabaseInfo> Databases { get; set; }
}

/// <summary>
/// 系统数据信息类。
/// </summary>
public class SystemDataInfo
{
    /// <summary>
    /// 取得或设置系统信息对象。
    /// </summary>
    public SystemInfo System { get; set; }

    /// <summary>
    /// 取得或设置系统运行时长。
    /// </summary>
    public double RunTime { get; set; }

    /// <summary>
    /// 取得或设置系统版本信息对象。
    /// </summary>
    public VersionInfo Version { get; set; }
}

/// <summary>
/// 定时任务摘要信息类。
/// </summary>
public class TaskSummaryInfo
{
    /// <summary>
    /// 取得或设置定时任务当前状态。
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// 取得或设置定时任务当前描述信息。
    /// </summary>
    public string Message { get; set; }
}