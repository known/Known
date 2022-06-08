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

#if !NET6_0
using System.Web.Routing;

namespace Known.Web
{
    class MvcRoute : Route
    {
        public MvcRoute(string url) : base(url, new MvcRouteHandler())
        {
        }

        public MvcRoute(string url, RouteValueDictionary defaults) : base(url, defaults, new MvcRouteHandler())
        {
        }
    }
}
#endif