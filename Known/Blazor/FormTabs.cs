﻿namespace Known.Blazor;

/// <summary>
/// 模块表单标签组件基类。
/// </summary>
public class ModuleFormTab : BaseComponent
{
    /// <summary>
    /// 取得或设置模块信息。
    /// </summary>
    [Parameter] public ModuleInfo Module { get; set; }
}

/// <summary>
/// 用户表单标签组件基类。
/// </summary>
public class UserFormTab : BaseComponent
{
    /// <summary>
    /// 取得或设置用户信息。
    /// </summary>
    [Parameter] public UserInfo User { get; set; }
}