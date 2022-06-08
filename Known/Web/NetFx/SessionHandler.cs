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
using System.Web;
using System.Web.SessionState;

namespace Known.Web
{
    class SessionHandler : IHttpHandler, IRequiresSessionState
    {
        public static readonly SessionHandler Instance = new SessionHandler();

        public void ProcessRequest(HttpContext context)
        {
        }

        public bool IsReusable { get; }
    }
}
#endif