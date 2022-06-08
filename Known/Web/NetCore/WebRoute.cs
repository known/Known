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

#if NET6_0
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;

namespace Known.Web;

class WebRoute : IRouter
{
    public Task RouteAsync(RouteContext context)
    {
        var url = context.HttpContext.Request.Path.Value;
        if (url == "/")
        {
            context.Handler = async ctx => {
                var response = ctx.Response;
                var html = WebUtils.GetIndexHtml(ctx.Request);
                var bytes = Encoding.UTF8.GetBytes(html);
                response.ContentType = MimeTypes.TextHtml;
                await response.Body.WriteAsync(bytes);
            };
        }
        return Task.CompletedTask;
    }

    public VirtualPathData GetVirtualPath(VirtualPathContext context)
    {
        return null;
    }
}
#endif