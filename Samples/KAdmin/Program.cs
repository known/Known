/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2022-06-28     KnownChen    ³õÊ¼»¯
 * ------------------------------------------------------------------------------- */

using KAdmin;
using Known.Razor;
using Known.Web;

KHost.Run(args, o =>
{
    o.IsBlazor = true;
    o.DbFactories["MySqlConnector"] = typeof(MySqlConnector.MySqlConnectorFactory);
    //o.Modules.Add(typeof(Known.Dev.AppModule));
    o.Injection = (services, a) =>
    {
        services.AddKBlazor();
        services.AddSingleton<DataService>();
    };
});
