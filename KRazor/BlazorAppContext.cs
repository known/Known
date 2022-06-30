/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2022-06-29     KnownChen    初始化
 * ------------------------------------------------------------------------------- */

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace Known.Razor;

internal class BlazorAppContext : AppContext
{
    [Inject] private ProtectedSessionStorage SessionStorage { get; set; }

    public override bool IsMobile { get; set; }

    public override string Host
    {
        get { return "localhost"; }
    }

    public override string GetIPAddress()
    {
        return string.Empty;
    }

    public override string GetRequest(string key)
    {
        return string.Empty;
    }

    public override T GetSession<T>(string key)
    {
        return SessionStorage.GetAsync<T>(key).Result.Value;
    }

    public override void SetSession(string key, object value)
    {
        SessionStorage.SetAsync(key, value);
    }

    public override void ClearSession()
    {
    }
}