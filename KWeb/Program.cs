/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * 2022-06-23     KnownChen    改成SQLite数据库
 * ------------------------------------------------------------------------------- */

using Known.Web;

KHost.Run(args, o =>
{
    o.DbFactories["SQLite"] = typeof(Microsoft.Data.Sqlite.SqliteFactory);
    o.Modules.Add(typeof(Known.Dev.AppModule));
});