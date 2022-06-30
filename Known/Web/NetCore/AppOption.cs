/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * 2022-06-16     KnownChen    Remove UseBlazorServer
 * ------------------------------------------------------------------------------- */

#if NET6_0
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Known.Web;

public class AppOption
{
    public AppOption()
    {
        Modules = new List<Type>();
        DbFactories = new Dictionary<string, Type>();
    }

    public bool IsBlazor { get; set; }
    public AppInfo App { get; set; }
    public List<Type> Modules { get; }
    public Dictionary<string, Type> DbFactories { get; }
    public Action<MvcOptions> MvcOption { get; set; }
    public Action<JsonOptions> JsonOption { get; set; }
    public Action<IServiceCollection, AppInfo> Injection { get; set; }
}
#endif