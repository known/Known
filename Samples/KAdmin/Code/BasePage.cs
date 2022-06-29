/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2022-06-28     KnownChen    初始化
 * ------------------------------------------------------------------------------- */

using Known.Razor;
using Microsoft.AspNetCore.Components;

namespace KAdmin;

public class BasePage : PageComponent
{
    [Inject] protected DataService Service { get; set; }

    protected bool IsAdmin
    {
        get { return Client.CheckIsAdmin(CurrentUser); }
    }
}
