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

using System.Collections.Generic;
using System.Linq;
using Known.Core;

namespace Known.Dev;

public class AppModule : IAppModule
{
    public void Initialize(AppInfo app)
    {
        PlatformAction.RegisterDictionary("DEV", (db, compNo) =>
        {
            var codes = new List<CodeInfo>();
            var service = new SystemService();
            var systems = service.GetSystems();
            if (systems != null && systems.Count > 0)
            {
                codes = systems.Select(d => new CodeInfo("ProductData", d.Code, d.Name, d)).ToList();
            }
            return codes;
        });
    }
}