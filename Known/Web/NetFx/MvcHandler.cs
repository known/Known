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
using System.Web.Routing;
using System.Web.SessionState;

namespace Known.Web
{
    class MvcHandler : IHttpHandler, IRequiresSessionState
    {
        public MvcHandler(RequestContext requestContext)
        {
            RequestContext = requestContext;
        }

        public RequestContext RequestContext { get; }
        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            ServiceHandler.Execute(new ActionContext(RequestContext, context));
        }
    }
}
#endif