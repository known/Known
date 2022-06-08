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
using System;
using System.Collections.Generic;
using System.Reflection;
using Known.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;

namespace Known.Web;

class ApiActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var action = context.ActionDescriptor as ControllerActionDescriptor;
        var attribute = action.MethodInfo.GetCustomAttribute(typeof(AnonymousAttribute), false);
        if (attribute != null)
            return;

        var user = UserHelper.GetUser(out string error);
        if (user == null)
        {
            context.Result = new JsonResult(new { timeout = true, msg = error });
            return;
        }

        var scheme = context.HttpContext.Request.Scheme;
        var host = context.HttpContext.Request.Host;
        var menus = UserHelper.GetMenus() ?? new List<MenuInfo>();
        menus.Add(new MenuInfo { Url = $"{scheme}://{host}/" });

        var referer = context.HttpContext.Request.Headers[HeaderNames.Referer].ToString();
        //var isAjaxCall = context.HttpContext.Request.Headers[HeaderNames.XRequestedWith] == "XMLHttpRequest";
        //var path = isAjaxCall ? referer : url;
        if (!string.IsNullOrWhiteSpace(referer))
        {
            if (!menus.Exists(m => !string.IsNullOrWhiteSpace(m.Url) && referer.StartsWith(m.Url)))
            {
                context.Result = new JsonResult(new { error = true, type = "403" });
                return;
            }
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        //context.HttpContext.Response.Headers.Add("Access-Control-Allow-Headers", "*");
        //context.HttpContext.Response.Headers.Add("Access-Control-Allow-Methods", "*");
        //context.HttpContext.Response.Headers.Add("Access-Control-Max-Age", new StringValues("24*60*60"));

        var result = context.Result;
        if (result is ObjectResult obj && obj.Value is ApiFile)
        {
            var file = obj.Value as ApiFile;
            var contentType = file.ContentType;
            if (!string.IsNullOrEmpty(file.FileName))
            {
                var key = Constants.KeyDownload;
                var token = context.HttpContext.Request.Form[key];
                context.HttpContext.Response.Cookies.Append(key, token);

                if (file.FileName.ToLower().EndsWith(".pdf"))
                    contentType = MimeTypes.ApplicationPdf;
                else if (file.FileName.ToLower().EndsWith(".xlsx"))
                    contentType = MimeTypes.ApplicationExcel;
            }

            var bytes = file.Bytes;
            if (bytes == null)
            {
                bytes = Array.Empty<byte>();
            }
            var type = new MediaTypeHeaderValue(contentType ?? MimeTypes.ApplicationOctetStream);
            context.Result = new FileContentResult(bytes, type)
            {
                FileDownloadName = file.FileName
            };
        }
    }
}
#endif