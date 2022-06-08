/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * ------------------------------------------------------------------------------- */

using Known.Web;

KHost.Run(args, o =>
{
    o.DbFactories["MySqlConnector"] = typeof(MySqlConnector.MySqlConnectorFactory);
});