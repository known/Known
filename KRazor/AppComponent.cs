/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2022-04-01     KnownChen
 * ------------------------------------------------------------------------------- */

using Microsoft.AspNetCore.Components;

namespace Known.Razor;

public class AppContext
{
    public bool IsMobile { get; set; }
    public DialogContainer Dialog { get; set; }
}

public abstract class AppComponent : BaseComponent
{
}
